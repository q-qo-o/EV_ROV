#include "sys.h"
#include "usart.h"	
#include "stm32f4xx.h"
#include "string.h"
#include "usart2_MEMS.h"
#include "math.h"
#include "string.h"
////////////////////////////////////////////////////////////////////////////////// 	 
//如果使用ucos,则包括下面的头文件即可.
#if SYSTEM_SUPPORT_OS
#include "includes.h"					//ucos 使用	  
#endif
//////////////////////////////////////////////////////////////////////////////////	 
//本程序只供学习使用，未经作者许可，不得用于其它任何用途
//ALIENTEK STM32F4探索者开发板
//串口1初始化		   
//正点原子@ALIENTEK
//技术论坛:www.openedv.com
//修改日期:2014/6/10
//版本：V1.5
//版权所有，盗版必究。
//Copyright(C) 广州市星翼电子科技有限公司 2009-2019
//All rights reserved
//********************************************************************************
//V1.3修改说明 
//支持适应不同频率下的串口波特率设置.
//加入了对printf的支持
//增加了串口接收命令功能.
//修正了printf第一个字符丢失的bug
//V1.4修改说明
//1,修改串口初始化IO的bug
//2,修改了USART_RX_STA,使得串口最大接收字节数为2的14次方
//3,增加了USART_REC_LEN,用于定义串口最大允许接收的字节数(不大于2的14次方)
//4,修改了EN_USART1_RX的使能方式
//V1.5修改说明     
//1,增加了对UCOSII的支持
////////////////////////////////////////////////////////////////////////////////// 	  


//////////////////////////////////////////////////////////////////
//加入以下代码,支持printf函数,而不需要选择use MicroLIB	  
#if 1
#pragma import(__use_no_semihosting)             
//标准库需要的支持函数                 
struct __FILE 
{ 
	int handle; 
}; 

FILE __stdout;       
//定义_sys_exit()以避免使用半主机模式    
int _sys_exit(int x) 
{ 
	x = x; 
} 
//重定义fputc函数 
int fputc(int ch, FILE *f)
{ 	
	while((USART1->SR&0X40)==0);//循环发送,直到发送完毕   
	USART1->DR = (u8) ch;      
	return ch;
}
#endif
 
//串口1中断服务程序
//注意,读取USARTx->SR能避免莫名其妙的错误 
//接收状态
//bit15，	接收完成标志
//bit14，	接收到0x0d
//bit13~0，	接收到的有效字节数目
u8  usart1_rebuf[40];      //接收上位机发过来数据
u8 PC_to_MCU[40]={0xFE,0xef,0x00,0x64,0x64,0x64,0x64};     //用于储存已经校验过的数据
u16 USART1_RX_STA=0;       //接收状态标记
u8 USART_RX_BUF[USART_REC_LEN];     //接收缓冲,最大USART_REC_LEN个字节.
u8  canbuf[8]={0xFA,0x64,0x64,0x64,0x64,0x64,0x64,0XFA};
u8 MCU_to_PC[24];                                         //发送给PC的数据

extern MEMS_struct mems; //姿态传感器出入的数据


//初始化IO 串口1 
//bound:波特率
void uart_DMA_init(u32 bound){
   //GPIO端口设置
   GPIO_InitTypeDef GPIO_InitStructure;
	USART_InitTypeDef USART_InitStructure;
	NVIC_InitTypeDef NVIC_InitStructure;
	DMA_InitTypeDef  DMA_InitStructure;
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOA,ENABLE); //使能GPIOA时钟
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART1,ENABLE);//使能USART1时钟
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_DMA2,ENABLE);//DMA1时钟使能 
 
	//串口1对应引脚复用映射
	GPIO_PinAFConfig(GPIOA,GPIO_PinSource9,GPIO_AF_USART1); //GPIOA9复用为USART1
	GPIO_PinAFConfig(GPIOA,GPIO_PinSource10,GPIO_AF_USART1); //GPIOA10复用为USART1
	
	//USART1端口配置
   GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9|GPIO_Pin_10 ; //GPIOA9与GPIOA10
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;//复用功能
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;	//速度50MHz
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP; //推挽复用输出
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP; //上拉
	GPIO_Init(GPIOA,&GPIO_InitStructure); //初始化PA9，PA10
		//USART1端口配置
   //USART1 初始化设置
	USART_InitStructure.USART_BaudRate = bound;//波特率设置
	USART_InitStructure.USART_WordLength = USART_WordLength_8b;//字长为8位数据格式
	USART_InitStructure.USART_StopBits = USART_StopBits_1;//一个停止位
	USART_InitStructure.USART_Parity = USART_Parity_No;//无奇偶校验位
	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;//无硬件数据流控制
	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;	//收发模式
  USART_Init(USART1, &USART_InitStructure); //初始化串口1
	//USART_ClearFlag(USART1, USART_FLAG_TC);
	NVIC_InitStructure.NVIC_IRQChannel = USART1_IRQn;//串口2中断通道
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0;//抢占优先级3 数值越小优先级越高
	NVIC_InitStructure.NVIC_IRQChannelSubPriority =3;//子优先级3,响应优先级
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;//IRQ通道使能
	NVIC_Init(&NVIC_InitStructure);//根据指定的参数初始化VIC寄存器、
	USART_ITConfig(USART1, USART_IT_RXNE, ENABLE);//开启接收中断
	USART_ITConfig(USART1, USART_IT_IDLE, ENABLE);//开启串口接受中断(帧中断)

	//开启DMA，串口以及中断
	USART_Cmd(USART1, ENABLE);	
}

void USART1_IRQHandler(void)
{
	 u8 clear=clear;
	 USART_ClearFlag(USART1,USART_FLAG_TC);
 
	 if(USART_GetITStatus(USART1,USART_IT_RXNE)!=RESET)        
	   {
           usart1_rebuf[USART1_RX_STA++]=USART1->DR;
	   }
		 
	 else if(USART_GetFlagStatus(USART1,USART_FLAG_IDLE)!=RESET)
	        {					
                clear=USART1->SR;
                clear=USART1->DR;	
								   //读取姿态数据
	              USART1_RX_STA=0;
       if(usart1_rebuf[0]==0xFE&&usart1_rebuf[1]==0xFE&&usart1_rebuf[39]==0xFD)   //判断数据是否有效
			 {
				 memcpy(PC_to_MCU,usart1_rebuf,40);
			 }
           }	
}
float main_to_motorf[10]={0xaf,0x64,0x64,0x64,0x64,0x00,0x00,0x00,0x00,0xaf};      //发送给电机的数据
u8 main_to_motoru8[10]={0xaf,0x64,0x64,0x64,0x64,0x00,0x00,0x00,0x00,0xaf}; 
//如果是四个电机1对应左主推，2对应右主推，3对应左垂推，4对应右垂推，5,6无效（从机器人尾向机器人头看）
//如果是六个电机1，2,对应左右主推，34对应前后侧推，56对应左右垂推
/*
功能：处理上位机发过来的电机数据
参数：void
返回值：void
*/
extern u8 speed_rata;
void motor_data_conduct(void)
{
	    u8 i=0;
//前进转向	


//				if(PC_to_MCU[4]-100>0)
//				{
//				motor_data[0]=(u8)(PC_to_MCU[3]-abs(PC_to_MCU[4]-100));
//				motor_data[1]=(u8)(PC_to_MCU[3]+abs(PC_to_MCU[4]-100));
//				}
//				else
//				{
//				motor_data[0]=(u8)(PC_to_MCU[3]-abs(PC_to_MCU[4]-100));
//				motor_data[1]=(u8)(PC_to_MCU[3]+abs(PC_to_MCU[4]-100));
//				}
		
      	if(PC_to_MCU[3]!=0x64)
				{
				main_to_motorf[1]=(float)PC_to_MCU[3]+(100-(float)PC_to_MCU[3])*((float)100-speed_rata)/100;
				main_to_motorf[2]=(float)PC_to_MCU[3]+(100-(float)PC_to_MCU[3])*((float)100-speed_rata)/100;
				}
			 if(PC_to_MCU[3]==0x64)
			 {
				 
				 if(PC_to_MCU[4]!=0x64)
				 {
					 	if(PC_to_MCU[4]>0x64)
					{
				main_to_motorf[1]=20;
				main_to_motorf[2]=180;
					}
					else
					{
						main_to_motorf[1]=180;
						main_to_motorf[2]=20;
					}
				}
				 else
				 {
					main_to_motorf[1]=100;
				  main_to_motorf[2]=100;
				 }
			 }				


				
			
//上浮下潜			
				main_to_motorf[3]=(float)PC_to_MCU[6]-(100-(float)PC_to_MCU[6])*((float)100-speed_rata)/100;
				main_to_motorf[4]=200-(float)PC_to_MCU[6]-(100-(float)PC_to_MCU[6])*((float)100-speed_rata)/100;
//舵机
				main_to_motorf[7]=(float)PC_to_MCU[7];
}
extern float real_deep;
/*
*功能：把数据发送给PC
参数：void
返回值：void
*/
u16 test1;
void send_to_PC(void)
{
	u16 temp;
  int i;
	MCU_to_PC[0]=0xFE;
	MCU_to_PC[1]=0xFE;	
   MCU_to_PC[22]=0xFD;
	MCU_to_PC[23]=0xFD;	
//roll
  if(mems.roll>0)
  {
	MCU_to_PC[2]=0x00;	
	temp=(u16)(mems.roll*100);
	MCU_to_PC[3]=temp>>8&0xff;	  //高八位
	MCU_to_PC[4]=temp&0xff;	      //低八位
  }
  else
  {
	MCU_to_PC[2]=0x01;	
	temp=pow(2,16)-(u16)(int16_t)(mems.roll*100);
	MCU_to_PC[3]=temp>>8&0xff;	  //高八位
	MCU_to_PC[4]=temp&0xff;	      //低八位
  }	  
//pitch
  if(mems.pitch>0)
  {
	MCU_to_PC[5]=0x00;	
	temp=(u16)(mems.pitch*100);
	MCU_to_PC[6]=temp>>8&0xff;	  //高八位
	MCU_to_PC[7]=temp&0xff;	      //低八位
  }
  else
  {
	MCU_to_PC[5]=0x01;	
	temp=pow(2,16)-(u16)(int16_t)(mems.pitch*100);
	MCU_to_PC[6]=temp>>8&0xff;	  //高八位
	MCU_to_PC[7]=temp&0xff;	      //低八位
  }	
//yaw
  if(mems.yaw>0)
  {
	MCU_to_PC[8]=0x00;	
	temp=(u16)(mems.yaw*100);
	MCU_to_PC[9]=temp>>8&0xff;	  //高八位
	MCU_to_PC[10]=temp&0xff;	      //低八位
  }
  else
  {
	MCU_to_PC[8]=0x01;	
	temp=pow(2,16)-(u16)(int16_t)(mems.yaw*100);
	MCU_to_PC[9]=temp>>8&0xff;	  //高八位
	MCU_to_PC[10]=temp&0xff;	      //低八位
  }	
//深度
	temp=abs((int)(real_deep*100));
	MCU_to_PC[11]=temp>>8&0xff;	  //高八位
	MCU_to_PC[12]=temp&0xff;	      //低八位
	test1=temp;
//校验位
	MCU_to_PC[21]=0xFE;       //从报文头开始
	for(i=1;i<21;i++)
  MCU_to_PC[21]^=MCU_to_PC[i];  //抑或校验
	for(i=0;i<24;i++)
	{
        while((USART1->SR&0X80)==0);
				  USART1->DR=MCU_to_PC[i];
	}
}
/*
功能：把b数组复制给a同时计算a的抑或校验值
参数
返回值
*/
void XOR_check(u8 a[],u8 b[])
{
	int i;
	u8 temp;
	a[0]=0XAF;
	for(i=1;i<7;i++)
	a[i]=b[i-1];
	a[7]=a[0];
	for(i=1;i<7;i++)
	a[7]^=a[i];
}
/*
功能：把float变量变成u8变量
参数;void
返回值：void
*/
void float_to_u8()                    //把float变量变成u8
{
	u8 i=0;
	for(i=0;i<10;i++)
	  main_to_motoru8[i]=(u8)main_to_motorf[i];
}
