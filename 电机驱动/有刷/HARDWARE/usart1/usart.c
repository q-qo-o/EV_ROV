#include "usart1.h"
#include "stm32f10x.h" // Device header
#include "string.h"
#include "delay.h"
#include "led.h"

u16 over_flag;

void usart1_init(void)
{
	GPIO_InitTypeDef GPIO_InitStructure;
	USART_InitTypeDef USART_InitStructure;
	NVIC_InitTypeDef NVIC_InitStructure;

	RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART1 | RCC_APB2Periph_GPIOA, ENABLE); //使能USART1，GPIOA时钟

	// USART1_TX   GPIOA.9
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9; // PA.9
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP; //复用推挽输出
	GPIO_Init(GPIOA, &GPIO_InitStructure);			//初始化GPIOA.9

	// USART1_RX	  GPIOA.10初始化
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10;			  // PA10
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING; //浮空输入
	GPIO_Init(GPIOA, &GPIO_InitStructure);				  //初始化GPIOA.10

	// Usart1 NVIC 配置
	NVIC_InitStructure.NVIC_IRQChannel = USART1_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1; //抢占优先级3
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 3;		  //子优先级3
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;			  // IRQ通道使能
	NVIC_Init(&NVIC_InitStructure);							  //根据指定的参数初始化VIC寄存器

	// USART 初始化设置

	USART_InitStructure.USART_BaudRate = 115200;									//串口波特率
	USART_InitStructure.USART_WordLength = USART_WordLength_8b;						//字长为8位数据格式
	USART_InitStructure.USART_StopBits = USART_StopBits_1;							//一个停止位
	USART_InitStructure.USART_Parity = USART_Parity_No;								//无奇偶校验位
	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None; //无硬件数据流控制
	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;					//收发模式

	USART_Init(USART1, &USART_InitStructure);	   //初始化串口1
	USART_ITConfig(USART1, USART_IT_RXNE, ENABLE); //开启串口接受中断
	USART_ITConfig(USART1, USART_IT_IDLE, ENABLE); //开启串口接受中断
	USART_Cmd(USART1, ENABLE);					   //使能串口1
}

u8 USART1_RX_BUF[10];
u8 recount;
u8 data_from_main[8] = {0x64, 0x64, 0x64, 0x64, 0x64, 0x64, 0x00, 0x00};
void USART1_IRQHandler(void) //串口1中断服务程序
{
	u8 clear = clear;
	if (USART_GetITStatus(USART1, USART_IT_RXNE) != RESET) //等待接收中断
	{
		USART1_RX_BUF[recount++] = USART_ReceiveData(USART1);
	}
	if (USART_GetITStatus(USART1, USART_IT_IDLE) != RESET)
	{
		clear = USART1->SR; //清除中断标志位
		clear = USART1->DR;
		if (USART1_RX_BUF[0] == 0xAF)
			memcpy(data_from_main, &USART1_RX_BUF[1], 8);
		LED0 = !LED0;
		recount = 0;
		over_flag = 0;
	}
}
/*
功能：将收到的数据转化，使pwm波占空比缓慢变化
参数：收到的数据，未转化的canbuf、canbuf1
返回值：void
*/
u16 motor_data[8] = {0x5DC, 0x5DC, 0x5DC, 0x5DC, 0x5DC, 0x5DC, 0x5DC, 0x5DC}; //把原始数据转化成相应的PWM波的值 6-8
u16 motor_pwm[8] = {0x5DC, 0x5DC, 0x5DC, 0x5DC, 0x5DC, 0x5DC, 0x5DC, 0x5DC};  //实际的PWM输出值，初始值为零电位 6-8
u16 servo_open = 800;
u16 servo_close = 2200;
u16 servo_pwm = 2200;

u16 servo_now_pwm = 2200;

void translate(void)
{
	u8 i;
	over_flag++;
	if (over_flag > 100)
	{
		//for (i = 0; i < 6; i++)
			//motor_data[i] = 0x5DC; // 5dc=1500
		data_from_main[0] = 100,   // 0x64=100
		data_from_main[1] = 100,
		data_from_main[2] = 100;
		data_from_main[3] = 100;
		data_from_main[6] = 0x00;
		over_flag = 100;
	}

	data_from_main[0] = data_from_main[0];
	data_from_main[1] = 200 - data_from_main[1];
	data_from_main[2] = data_from_main[2];
	data_from_main[3] = data_from_main[3];

	motor_data[0] = (int)(max - (data_from_main[0] * max / 200)); //将0~200之间的值转化成0~max
	motor_data[3] = (int)(max - (data_from_main[1] * max / 200)); //将0~200之间的值转化成0~max
	motor_data[1] = (int)(max - (data_from_main[2] * max / 200)); //将0~200之间的值转化成0~max
	motor_data[2] = (int)(max - (data_from_main[3] * max / 200)); //将0~200之间的值转化成0~max

	for (i = 0; i < 4; i++) //前6个数值分给电机
	{
		//电机限幅
		if (motor_data[i] > max) // 76c=1900，44c=1100，FA0=max
			motor_data[i] = max;
		//if (motor_data[i] < 0)
			//motor_data[i] = 0;
	}
	//缓慢变速，防止瞬时大电流导致电调烧毁或电源断电或电机烧毁
	//电机1缓慢变速
	if (abs(motor_data[0] - motor_pwm[0]) > 10)
	{
		if (motor_data[0] < motor_pwm[0])
			motor_pwm[0] -= (int)abs(speed*(motor_data[0] - motor_pwm[0]));
		if (motor_data[0] > motor_pwm[0])
			motor_pwm[0] += (int)abs(speed*(motor_data[0] - motor_pwm[0]));
	}
	//else
		//motor_pwm[0] = motor_data[0];
	//电机2缓慢变速
	if (abs(motor_data[1] - motor_pwm[1]) > 10)
	{
		if (motor_data[1] < motor_pwm[1])
			motor_pwm[1] -= (int)abs(speed*(motor_data[1] - motor_pwm[1]));
		if (motor_data[1] > motor_pwm[1])
			motor_pwm[1] += (int)abs(speed*(motor_data[1] - motor_pwm[1]));
	}
	//else
		//motor_pwm[1] = motor_data[1];
	//电机3缓慢变速
	if (abs(motor_data[2] - motor_pwm[2]) > 10)
	{
		if (motor_data[2] < motor_pwm[2])
			motor_pwm[2] -= (int)abs(speed*(motor_data[2] - motor_pwm[2]));
		if (motor_data[2] > motor_pwm[2])
			motor_pwm[2] += (int)abs(speed*(motor_data[2] - motor_pwm[2]));
	}
	//else
		//motor_pwm[2] = motor_data[2];
	//电机4缓慢变速
	if (abs(motor_data[3] - motor_pwm[3]) > 10)
	{
		if (motor_data[3] < motor_pwm[3])
			motor_pwm[3] -= (int)abs(speed*(motor_data[3] - motor_pwm[3]));
		if (motor_data[3] > motor_pwm[3])
			motor_pwm[3] += (int)abs(speed*(motor_data[3] - motor_pwm[3]));
	}
	//else
		//motor_pwm[3] = motor_data[3];




	//设置占空比 1
	if (motor_pwm[0] > max / 2)
	{
		moter1 = (int)abs(2 * (motor_pwm[0] - max / 2));
		moter2 = 0;
	}
	else if (motor_pwm[0] < max / 2)
	{
		moter2 = (int)abs(2 * (motor_pwm[0] - max / 2));
		moter1 = 0;
	}
	else
	{
		moter1 = moter2 = 0;
	}
	//设置占空比 2
	if (motor_pwm[1] > max / 2)
	{
		moter3 = (int)abs(2 * (motor_pwm[1] - max / 2));
		moter4 = 0;
	}
	else if (motor_pwm[1] < max / 2)
	{
		moter4 = (int)abs(2 * (motor_pwm[1] - max / 2));
		moter3 = 0;
	}
	else
	{
		moter3 = moter4 = 0;
	}
	//设置占空比 3
	if (motor_pwm[2] > max / 2)
	{
		moter5 = (int)abs(2 * (motor_pwm[2] - max / 2));
		moter6 = 0;
	}
	else if (motor_pwm[2] < max / 2)
	{
		moter6 = (int)abs(2 * (motor_pwm[2] - max / 2));
		moter5 = 0;
	}
	else
	{
		moter5 = moter6 = 0;
	}

	if (motor_pwm[3] > max / 2)
	{
		moter7 = (int)abs(2 * (motor_pwm[3] - max / 2));
		moter8 = 0;
	}
	else if (motor_pwm[3] < max / 2)
	{
		moter8 = (int)abs(2 * (motor_pwm[3] - max / 2));
		moter7 = 0;
	}
	else
	{
		moter7 = moter8 = 0;
	}

	/*
		moter1 = motor_pwm[0];
		moter2 = max - motor_pwm[0];
		moter3 = motor_pwm[1];
		moter4 = max - motor_pwm[1];
		moter5 = motor_pwm[2];
		moter6 = max - motor_pwm[2];
		moter7 = motor_pwm[3];
		moter8 = max - motor_pwm[3];
	*/

	// 1.5 2.6 3.8 4.7

	//*******************   舵机  ******************************************

	if (data_from_main[6] == 0 && servo_now_pwm <= 2200) //关闭
		servo_now_pwm += 100;

	if (data_from_main[6] == 0 && servo_now_pwm == 2000)
	{
		while (servo_pwm < 2200)
		{
			servo_pwm += 20;
			servo_now_pwm += 20;
		}
	}

	if (data_from_main[6] == 1 && servo_now_pwm >= 1200) //打开
		servo_now_pwm -= 100;

	if (data_from_main[6] == 1 && servo_now_pwm == 1400)
	{
		while (servo_pwm > 1200)
		{
			servo_pwm -= 20;
		}
		TIM2->CCR3 = servo_pwm;
		delay_ms(500);
		while (servo_pwm < 1400)
		{
			servo_pwm += 20;
		}
		TIM2->CCR3 = servo_pwm;
	}

	TIM2->CCR3 = servo_pwm;
}
//功能：初始化电机、舵机
//参数：void
void MOTOR_Init(void)
{
	////初始化所有通道(电机+舵机)占空比：1500/19999
	TIM_SetCompare1(TIM4, 0); // PB6
	TIM_SetCompare2(TIM4, 0); // PB7
	TIM_SetCompare3(TIM4, 0); // PB8
	TIM_SetCompare4(TIM4, 0); // PB9
	TIM_SetCompare1(TIM3, 0); // PB6
	TIM_SetCompare2(TIM3, 0); // PB7
	TIM_SetCompare3(TIM3, 0); // PB8
	TIM_SetCompare4(TIM3, 0); // PB9
}
