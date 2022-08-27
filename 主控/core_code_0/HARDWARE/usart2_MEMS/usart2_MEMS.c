#include "usart2_MEMS.h"
#include "stm32f4xx.h"                  // Device header
u8 mems_rebuf[41];    //接收mems数组
u8 USART2_RX_STA;     //状态标志位
float MEMS_data[9];   //
/*
功能：MEMs串口的初始化
参数：void
返回值：void
*/
void usart2_init(void)
{
    //GPIO端口设置
	GPIO_InitTypeDef GPIO_InitStructure;
	USART_InitTypeDef USART_InitStructure;
	NVIC_InitTypeDef NVIC_InitStructure;
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOA,ENABLE); 
	//使能GPIOA时钟
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART2,ENABLE);//使能USART2时钟 
 //串口1对应引脚复用映射
	GPIO_PinAFConfig(GPIOA,GPIO_PinSource2,GPIO_AF_USART2); //GPIOA2复用为	USART2
	GPIO_PinAFConfig(GPIOA,GPIO_PinSource3,GPIO_AF_USART2); //GPIOA3复用为USART2
	//USART2端口配置
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_2 | GPIO_Pin_3; //GPIOA2与GPIOA3
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;//复用功能
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;//速度50MHz
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP; //推挽复用输出
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP; //上拉
	GPIO_Init(GPIOA,&GPIO_InitStructure); //初始化PA2，PA3
   //USART2 初始化设置
	USART_InitStructure.USART_BaudRate = 115200;//波特率设置
	USART_InitStructure.USART_WordLength = USART_WordLength_8b;//字长为8位数据格式
	USART_InitStructure.USART_StopBits = USART_StopBits_1;//一个停止位
	USART_InitStructure.USART_Parity = USART_Parity_No;//无奇偶校验位
	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;//无硬件数据流控制
	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;//收发模式
	USART_Init(USART2, &USART_InitStructure); //初始化串口2
	
	
	NVIC_InitStructure.NVIC_IRQChannel = USART2_IRQn;//串口2中断通道
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0;//抢占优先级3 数值越小优先级越高
	NVIC_InitStructure.NVIC_IRQChannelSubPriority =3;//子优先级3,响应优先级
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;//IRQ通道使能
	NVIC_Init(&NVIC_InitStructure);//根据指定的参数初始化VIC寄存器、
	USART_ITConfig(USART2, USART_IT_RXNE, ENABLE);//开启接收中断
	USART_ITConfig(USART2, USART_IT_IDLE, ENABLE);//开启串口接受中断(帧中断)
	USART_Cmd(USART2, ENABLE);  //使能串口2
}
//串口2帧中断
void USART2_IRQHandler(void)
{
	 u8 clear=clear;
	 USART_ClearFlag(USART2,USART_FLAG_TC);
 
	 if(USART_GetITStatus(USART2,USART_IT_RXNE)!=RESET)        
	   {
           mems_rebuf[USART2_RX_STA++]=USART2->DR;
	   }
		 
	 else if(USART_GetFlagStatus(USART2,USART_FLAG_IDLE)!=RESET)
	        {					
                clear=USART2->SR;
                clear=USART2->DR;	
								   //读取姿态数据
	              USART2_RX_STA=0;
           }	
}
/*
功能：处理MEMS的数据
参数：void
返回值：void
*/
MEMS_struct mems;
void MEMS_DATA_conduct(void)
{
	  if(mems_rebuf[0]==0xFA&&mems_rebuf[1]==0xAF&&mems_rebuf[39]==0xFB&&mems_rebuf[40]==0xbf)		
     memcpy(MEMS_data,&mems_rebuf[2],36);	
		 mems.roll=MEMS_data[0];
		 mems.pitch=MEMS_data[1];
		 mems.yaw=MEMS_data[2];
		 mems.roll_rate=MEMS_data[3];
		 mems.pitch_rate=MEMS_data[4];
		 mems.yaw_rate=MEMS_data[5];
		 mems.x_acc=MEMS_data[6];
		 mems.y_acc=MEMS_data[7];
		 mems.z_acc=MEMS_data[8];

		
}
