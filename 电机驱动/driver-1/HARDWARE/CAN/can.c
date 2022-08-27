#include "can.h"
#include "delay.h"
#include "usart.h"
#include "led.h"
#include "math.h"
#include "timer.h"
#include "string.h"
//////////////////////////////////////////////////////////////////////////////////	 
//本程序只供学习使用，未经作者许可，不得用于其它任何用途
//ALIENTEK战舰STM32开发板
//CAN驱动 代码	   
//正点原子@ALIENTEK
//技术论坛:www.openedv.com
//创建日期:2014/5/7
//版本：V1.1 
//版权所有，盗版必究。			

//Copyright(C) 广州市星翼电子科技有限公司 2014-2024
//All rights reserved	
//********************************************************************************
//V1.1修改说明 20150528
//修正了CAN初始化函数的相关注释，更正了波特率计算公式
////////////////////////////////////////////////////////////////////////////////// 	 
 
//CAN初始化
//tsjw:重新同步跳跃时间单元.范围:CAN_SJW_1tq~ CAN_SJW_4tq
//tbs2:时间段2的时间单元.   范围:CAN_BS2_1tq~CAN_BS2_8tq;
//tbs1:时间段1的时间单元.   范围:CAN_BS1_1tq ~CAN_BS1_16tq
//brp :波特率分频器.范围:1~1024;  tq=(brp)*tpclk1
//波特率=Fpclk1/((tbs1+1+tbs2+1+1)*brp);
//mode:CAN_Mode_Normal,普通模式;CAN_Mode_LoopBack,回环模式;
//Fpclk1的时钟在初始化的时候设置为36M,如果设置CAN_Mode_Init(CAN_SJW_1tq,CAN_BS2_8tq,CAN_BS1_9tq,4,CAN_Mode_LoopBack);
//则波特率为:36M/((8+9+1)*4)=500Kbps
//返回值:0,初始化OK;
//    其他,初始化失败; 
u8 CAN_Mode_Init(u8 tsjw,u8 tbs2,u8 tbs1,u16 brp,u8 mode)
{ 
	GPIO_InitTypeDef 		GPIO_InitStructure; 
	CAN_InitTypeDef        	CAN_InitStructure;
	CAN_FilterInitTypeDef  	CAN_FilterInitStructure;
#if CAN_RX0_INT_ENABLE 
	NVIC_InitTypeDef  		NVIC_InitStructure;
#endif

	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOA|RCC_APB2Periph_AFIO, ENABLE);//使能PORTA时钟	                   											 

	RCC_APB1PeriphClockCmd(RCC_APB1Periph_CAN1, ENABLE);//使能CAN1时钟	

	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_12;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;	//复用推挽
	GPIO_Init(GPIOA, &GPIO_InitStructure);			//初始化IO

	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_11;
		GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_Mode =  GPIO_Mode_IPU;//GPIO_Mode_IPU;	//上拉输入
	GPIO_Init(GPIOA, &GPIO_InitStructure);			//初始化IO

	//CAN单元设置
	CAN_InitStructure.CAN_TTCM=DISABLE;			//非时间触发通信模式  
	CAN_InitStructure.CAN_ABOM=DISABLE;			//软件自动离线管理	 
	CAN_InitStructure.CAN_AWUM=DISABLE;			//睡眠模式通过软件唤醒(清除CAN->MCR的SLEEP位)
	CAN_InitStructure.CAN_NART=ENABLE;			//禁止报文自动传送 
	CAN_InitStructure.CAN_RFLM=DISABLE;		 	//报文不锁定,新的覆盖旧的  
	CAN_InitStructure.CAN_TXFP=DISABLE;			//优先级由报文标识符决定 
	CAN_InitStructure.CAN_Mode= mode;	        //模式设置： mode:0,普通模式;1,回环模式; 
	//设置波特率
	CAN_InitStructure.CAN_SJW=tsjw;				//重新同步跳跃宽度(Tsjw)为tsjw+1个时间单位  CAN_SJW_1tq	 CAN_SJW_2tq CAN_SJW_3tq CAN_SJW_4tq
	CAN_InitStructure.CAN_BS1=tbs1; 			//Tbs1=tbs1+1个时间单位CAN_BS1_1tq ~CAN_BS1_16tq
	CAN_InitStructure.CAN_BS2=tbs2;				//Tbs2=tbs2+1个时间单位CAN_BS2_1tq ~	CAN_BS2_8tq
	CAN_InitStructure.CAN_Prescaler=brp;        //分频系数(Fdiv)为brp+1	
	CAN_Init(CAN1, &CAN_InitStructure);        	//初始化CAN1 

	CAN_FilterInitStructure.CAN_FilterNumber=0;	//过滤器0
	CAN_FilterInitStructure.CAN_FilterMode=CAN_FilterMode_IdMask; 	//屏蔽位模式
	CAN_FilterInitStructure.CAN_FilterScale=CAN_FilterScale_32bit; 	//32位宽 
	CAN_FilterInitStructure.CAN_FilterIdHigh=((0xE1<<21)&0xffff0000)>>16;	//32位ID
	CAN_FilterInitStructure.CAN_FilterIdLow=((0xE1<<21)|CAN_Id_Standard|CAN_RTR_Data)&0xffff;
	CAN_FilterInitStructure.CAN_FilterMaskIdHigh=0xFFFF;//32位MASK
	CAN_FilterInitStructure.CAN_FilterMaskIdLow=0xFFFF;
	CAN_FilterInitStructure.CAN_FilterFIFOAssignment=CAN_Filter_FIFO0;//过滤器0关联到FIFO0
	CAN_FilterInitStructure.CAN_FilterActivation=ENABLE;//激活过滤器0
  CAN_FilterInit(&CAN_FilterInitStructure);			//滤波器初始化
	
	CAN_FilterInitStructure.CAN_FilterNumber=1;	//过滤器1
	CAN_FilterInitStructure.CAN_FilterMode=CAN_FilterMode_IdMask; 	//屏蔽位模式
	CAN_FilterInitStructure.CAN_FilterScale=CAN_FilterScale_32bit; 	//32位宽 
	CAN_FilterInitStructure.CAN_FilterIdHigh=((0xE2<<21)&0xffff0000)>>16;	//32位ID
	CAN_FilterInitStructure.CAN_FilterIdLow=((0xE2<<21)|CAN_Id_Standard|CAN_RTR_Data)&0xffff;
	CAN_FilterInitStructure.CAN_FilterMaskIdHigh=0xFFFF;//32位MASK
	CAN_FilterInitStructure.CAN_FilterMaskIdLow=0xFFFF;
	CAN_FilterInitStructure.CAN_FilterFIFOAssignment=CAN_Filter_FIFO1;//过滤器0关联到FIFO1
	CAN_FilterInitStructure.CAN_FilterActivation=ENABLE;//激活过滤器1
  CAN_FilterInit(&CAN_FilterInitStructure);			//滤波器初始化
	
#if CAN_RX0_INT_ENABLE 
	CAN_ITConfig(CAN1,CAN_IT_FMP0,ENABLE);				//FIFO0消息挂号中断允许.		    

	NVIC_InitStructure.NVIC_IRQChannel = USB_LP_CAN1_RX0_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;     // 主优先级为1
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;            // 次优先级为0
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
#endif
	return 0;
}   
 
#if CAN_RX0_INT_ENABLE	//使能RX0中断
//中断服务函数			    
void USB_LP_CAN1_RX0_IRQHandler(void)
{
  	CanRxMsg RxMessage;
	int i=0;
    CAN_Receive(CAN1, 0, &RxMessage);
	for(i=0;i<8;i++)
	printf("rxbuf[%d]:%d\r\n",i,RxMessage.Data[i]);
}
#endif

//can发送一组数据(固定格式:ID为0X12,标准帧,数据帧)	
//len:数据长度(最大为8)				     
//msg:数据指针,最大为8个字节.
//返回值:0,成功;
//		 其他,失败;
u8 Can_Send_Msg(u8* msg,u8 len)
{	
	u8 mbox;
	u16 i=0;
	CanTxMsg TxMessage;
	TxMessage.StdId=0xE1;			// 标准标识符 
	TxMessage.IDE=CAN_Id_Standard; 	// 标准帧
	TxMessage.RTR=CAN_RTR_Data;		// 数据帧
	TxMessage.DLC=len;				// 要发送的数据长度
	for(i=0;i<len;i++)
	TxMessage.Data[i]=msg[i];			          
	mbox= CAN_Transmit(CAN1, &TxMessage);   
	i=0; 
	while((CAN_TransmitStatus(CAN1, mbox)==CAN_TxStatus_Failed)&&(i<0XFFF))i++;	//等待发送结束
	if(i>=0XFFF)return 1;
	return 0;	 
}
u8 Can_Send_Msg1(u8* msg,u8 len)
{	
	u8 mbox;
	u16 i=0;
	CanTxMsg TxMessage;
	TxMessage.StdId=0xE2;			// 标准标识符 
	TxMessage.RTR=CAN_RTR_Data;		// 数据帧
	TxMessage.DLC=len;				// 要发送的数据长度
	for(i=0;i<len;i++)
	TxMessage.Data[i]=msg[i];			          
	mbox= CAN_Transmit(CAN1, &TxMessage);   
	i=0; 
	while((CAN_TransmitStatus(CAN1, mbox)==CAN_TxStatus_Failed)&&(i<0XFFF))i++;	//等待发送结束
	if(i>=0XFFF)return 1;
	return 0;	 
}
/*
功能：  亦或校验
参数：  u8 *data   需要亦或校验的数组
        u16 length 校验的数据长度
返回值：u8 亦或校验的结果
*/
u8 xrl(u8 *data, u16 length)
{
   u8 *buff = data+1;
   u8 XOR_Check = 0;
   while(length-1)  
   {
       XOR_Check ^= *(buff++);
       length--;
   } 
	return XOR_Check;
}
//can口接收数据查询
//buf:数据缓存区;	 
//返回值:0,无数据被收到;
//		 其他,接收的数据长度;
u8 FIFO0_rebuf[8];       //FIFO0接收数据
u8 over_flag;
u8 Can_Receive_Msg(void)
{		   		   
 	  u32 i;
	  CanRxMsg RxMessage;
    if( CAN_MessagePending(CAN1,CAN_FIFO0)==0)return 0;		//没有接收到数据,直接退出 
    over_flag=0;
    CAN_Receive(CAN1, CAN_FIFO0, &RxMessage);//读取数据	
		if((RxMessage.Data[0]!=0xFA))return 0;//异或校验失败，直接退出
		memcpy(FIFO0_rebuf,RxMessage.Data,8);
		LED0=!LED0;//校验成功LED灯闪烁
	  return RxMessage.DLC;
}
u8 FIFO1_rebuf[8];       //FIFO1接收数据
u8 Can_Receive_Msg1(void)
{		   		   
//	u32 flag;
	  CanRxMsg RxMessage;
    if( CAN_MessagePending(CAN1,CAN_FIFO1)==0)return 0;		//没有接收到数据,直接退出 
	  over_flag=0;
    CAN_Receive(CAN1, CAN_FIFO1, &RxMessage);//读取数据
		if((RxMessage.Data[0]!=0xFE)||(RxMessage.Data[7]!=xrl(RxMessage.Data, 7)))return 0;//异或校验失败，直接退出
		memcpy(FIFO0_rebuf,RxMessage.Data,8);
		LED0=!LED0;//校验成功LED灯闪烁
	  return RxMessage.DLC;
}

//u8 xrl(u8 *data, u16 length)
//{
//   u8 *buff = data;
//   u8 XOR_Check = 0;
//   while(length)  
//   {
//       XOR_Check ^= *(buff++);
//       length--;
//   } 
//	return XOR_Check;
//}


/*
功能：将收到的数据转化，使pwm波占空比缓慢变化
参数：收到的数据，未转化的canbuf、canbuf1
返回值：void
*/
u16 motor_data[6]={0x5DC,0x5DC,0x5DC,0x5DC,0x5DC,0x5DC};   //把原始数据转化成相应的PWM波的值
u16 motor_pwm[6]={0x5DC,0x5DC,0x5DC,0x5DC,0x5DC,0x5DC};    //实际的PWM输出值，初始值为零电位
void translate(void)
{
	   u8 i;
	   over_flag++;
   	if(over_flag>100)
		{
			for(i=0;i<6;i++)
			  motor_data[i]=0x5DC;
			over_flag=100;
		}
			for(i=0;i<6;i++)                               //前6个数值分给电机
			{
				if(FIFO0_rebuf[0]==0xFA)
				{
				motor_data[i]=0x44C+(FIFO0_rebuf[i+1]*4);  //将0~200之间的值转化成1100~1900
				//电机限幅
			  if(motor_data[i]>0x76C)
				motor_data[i]=0x76C;
			  if(motor_data[i]<0x44C)
				motor_data[i]=0x44C;
		  	}
			}
			for(i=1;i<7;i++)       //后2个数值分给舵机
			{
				FIFO1_rebuf[i]*=0x0A;      //将主控发过来的数据乘以10作为比较值
			}
//缓慢变速，防止瞬时大电流导致电调烧毁或者电源断电
//电机1缓慢变速
			if(abs(motor_data[0]-motor_pwm[0])>0x0a)
			{
				if(motor_data[0]<motor_pwm[0])
			   motor_pwm[0]-=0x0a;		 
			  if(motor_data[0]>motor_pwm[0])
			   motor_pwm[0]+=0x0a;
			}	
			else
				motor_pwm[0]=motor_data[0];
//电机2缓慢变速			
			if(abs(motor_data[1]-motor_pwm[1])>0x0a)
			{
				if(motor_data[1]<motor_pwm[1])
			   motor_pwm[1]-=0x0a;
			  if(motor_data[1]>motor_pwm[1])
				 motor_pwm[1]+=0x0a;
		  }	
			else
				motor_pwm[1]=motor_data[1];
//电机3缓慢变速			
			if(abs(motor_data[2]-motor_pwm[2])>0x0a)
			{
				if(motor_data[2]<motor_pwm[2])
			   motor_pwm[2]-=0x0a;			    
			  if(motor_data[2]>motor_pwm[2])
				motor_pwm[2]+=0x0a;
		  }	
			else
				motor_pwm[2]=motor_data[2];
//电机4缓慢变速			
			if(abs(motor_data[3]-motor_pwm[3])>0x0a)
			{
				if(motor_data[3]<motor_pwm[3])
			   motor_pwm[3]-=0x0a;			   
			  if(motor_data[3]>motor_pwm[3])
				 motor_pwm[3]+=0x0a;
		  }	
			else
				motor_pwm[3]=motor_data[3];

	
				
//		TIM_SetCompare1(TIM3,motor_pwm[0]);  //PA6 舵机1		
//		TIM_SetCompare2(TIM3,motor_pwm[1]);	//PA7  舵机2
//		TIM_SetCompare3(TIM3,canbuf[3]);  //PB0(备用)
////			TIM_SetCompare4(TIM3,canbuf[4]);  //PB1(备用)
//主推			
		  	moter2=motor_pwm[0];  //PB6	电机1
			  moter4=motor_pwm[1];	//PB7	电机2 
//垂推
		  	moter1=3000-motor_pwm[3];	//PB8 电机3
			  moter3 = motor_pwm[2];	//PB9 电机4		
//	//测试用
//			TIM_SetCompare1(TIM4,1600);  //PB6	电机1
//			TIM_SetCompare2(TIM4,1600);	//PB7	电机2 

//			TIM_SetCompare3(TIM4,1600);	//PB8 电机3
//			TIM_SetCompare4(TIM4,1600);	//PB9 电机4	
	}
//功能：初始化电机、舵机
//参数：void
void MOTOR_Init(void)
{
////初始化所有通道(电机+舵机)占空比：1500/19999
//	TIM_SetCompare1(TIM4,1900);  //PB6		
//	TIM_SetCompare2(TIM4,1900);	//PB7	 
//	TIM_SetCompare3(TIM4,1900);	//PB8
//	TIM_SetCompare4(TIM4,1900);	//PB9
//	 
//	 delay_ms(1000);
//	 delay_ms(1000);
//	 delay_ms(1000); 
//	TIM_SetCompare1(TIM4,1100);  //PB6		
//	TIM_SetCompare2(TIM4,1100);	//PB7	 
//	TIM_SetCompare3(TIM4,1100);	//PB8
//	TIM_SetCompare4(TIM4,1100);	//PB9	
//	delay_ms(1000); 
//	delay_ms(1000); 
	TIM_SetCompare1(TIM4,1500);  //PB6		
	TIM_SetCompare2(TIM4,1500);	//PB7	 

	TIM_SetCompare3(TIM4,1500);	//PB8
	TIM_SetCompare4(TIM4,1500);	//PB9
	 delay_ms(1000);
	 delay_ms(1000); 
	 delay_ms(1000);
	 delay_ms(1000); 
//	 delay_ms(1000); 
//	 delay_ms(1000);
//	 delay_ms(1000) ; 
//	 	 delay_ms(1000); 
//	 delay_ms(1000); 
//	 delay_ms(1000); 
}


	
