#include "sys.h"
#include "usart.h"	
#include "stm32f4xx.h"
#include "string.h"
#include "usart2_MEMS.h"
#include "math.h"
#include "string.h"
////////////////////////////////////////////////////////////////////////////////// 	 
//���ʹ��ucos,����������ͷ�ļ�����.
#if SYSTEM_SUPPORT_OS
#include "includes.h"					//ucos ʹ��	  
#endif
//////////////////////////////////////////////////////////////////////////////////	 
//������ֻ��ѧϰʹ�ã�δ��������ɣ��������������κ���;
//ALIENTEK STM32F4̽���߿�����
//����1��ʼ��		   
//����ԭ��@ALIENTEK
//������̳:www.openedv.com
//�޸�����:2014/6/10
//�汾��V1.5
//��Ȩ���У�����ؾ���
//Copyright(C) ������������ӿƼ����޹�˾ 2009-2019
//All rights reserved
//********************************************************************************
//V1.3�޸�˵�� 
//֧����Ӧ��ͬƵ���µĴ��ڲ���������.
//�����˶�printf��֧��
//�����˴��ڽ��������.
//������printf��һ���ַ���ʧ��bug
//V1.4�޸�˵��
//1,�޸Ĵ��ڳ�ʼ��IO��bug
//2,�޸���USART_RX_STA,ʹ�ô����������ֽ���Ϊ2��14�η�
//3,������USART_REC_LEN,���ڶ��崮�����������յ��ֽ���(������2��14�η�)
//4,�޸���EN_USART1_RX��ʹ�ܷ�ʽ
//V1.5�޸�˵��     
//1,�����˶�UCOSII��֧��
////////////////////////////////////////////////////////////////////////////////// 	  


//////////////////////////////////////////////////////////////////
//�������´���,֧��printf����,������Ҫѡ��use MicroLIB	  
#if 1
#pragma import(__use_no_semihosting)             
//��׼����Ҫ��֧�ֺ���                 
struct __FILE 
{ 
	int handle; 
}; 

FILE __stdout;       
//����_sys_exit()�Ա���ʹ�ð�����ģʽ    
int _sys_exit(int x) 
{ 
	x = x; 
} 
//�ض���fputc���� 
int fputc(int ch, FILE *f)
{ 	
	while((USART1->SR&0X40)==0);//ѭ������,ֱ���������   
	USART1->DR = (u8) ch;      
	return ch;
}
#endif
 
//����1�жϷ������
//ע��,��ȡUSARTx->SR�ܱ���Ī������Ĵ��� 
//����״̬
//bit15��	������ɱ�־
//bit14��	���յ�0x0d
//bit13~0��	���յ�����Ч�ֽ���Ŀ
u8  usart1_rebuf[40];      //������λ������������
u8 PC_to_MCU[40]={0xFE,0xef,0x00,0x64,0x64,0x64,0x64};     //���ڴ����Ѿ�У���������
u16 USART1_RX_STA=0;       //����״̬���
u8 USART_RX_BUF[USART_REC_LEN];     //���ջ���,���USART_REC_LEN���ֽ�.
u8  canbuf[8]={0xFA,0x64,0x64,0x64,0x64,0x64,0x64,0XFA};
u8 MCU_to_PC[24];                                         //���͸�PC������

extern MEMS_struct mems; //��̬���������������


//��ʼ��IO ����1 
//bound:������
void uart_DMA_init(u32 bound){
   //GPIO�˿�����
   GPIO_InitTypeDef GPIO_InitStructure;
	USART_InitTypeDef USART_InitStructure;
	NVIC_InitTypeDef NVIC_InitStructure;
	DMA_InitTypeDef  DMA_InitStructure;
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOA,ENABLE); //ʹ��GPIOAʱ��
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART1,ENABLE);//ʹ��USART1ʱ��
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_DMA2,ENABLE);//DMA1ʱ��ʹ�� 
 
	//����1��Ӧ���Ÿ���ӳ��
	GPIO_PinAFConfig(GPIOA,GPIO_PinSource9,GPIO_AF_USART1); //GPIOA9����ΪUSART1
	GPIO_PinAFConfig(GPIOA,GPIO_PinSource10,GPIO_AF_USART1); //GPIOA10����ΪUSART1
	
	//USART1�˿�����
   GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9|GPIO_Pin_10 ; //GPIOA9��GPIOA10
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;//���ù���
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;	//�ٶ�50MHz
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP; //���츴�����
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP; //����
	GPIO_Init(GPIOA,&GPIO_InitStructure); //��ʼ��PA9��PA10
		//USART1�˿�����
   //USART1 ��ʼ������
	USART_InitStructure.USART_BaudRate = bound;//����������
	USART_InitStructure.USART_WordLength = USART_WordLength_8b;//�ֳ�Ϊ8λ���ݸ�ʽ
	USART_InitStructure.USART_StopBits = USART_StopBits_1;//һ��ֹͣλ
	USART_InitStructure.USART_Parity = USART_Parity_No;//����żУ��λ
	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;//��Ӳ������������
	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;	//�շ�ģʽ
  USART_Init(USART1, &USART_InitStructure); //��ʼ������1
	//USART_ClearFlag(USART1, USART_FLAG_TC);
	NVIC_InitStructure.NVIC_IRQChannel = USART1_IRQn;//����2�ж�ͨ��
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0;//��ռ���ȼ�3 ��ֵԽС���ȼ�Խ��
	NVIC_InitStructure.NVIC_IRQChannelSubPriority =3;//�����ȼ�3,��Ӧ���ȼ�
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;//IRQͨ��ʹ��
	NVIC_Init(&NVIC_InitStructure);//����ָ���Ĳ�����ʼ��VIC�Ĵ�����
	USART_ITConfig(USART1, USART_IT_RXNE, ENABLE);//���������ж�
	USART_ITConfig(USART1, USART_IT_IDLE, ENABLE);//�������ڽ����ж�(֡�ж�)

	//����DMA�������Լ��ж�
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
								   //��ȡ��̬����
	              USART1_RX_STA=0;
       if(usart1_rebuf[0]==0xFE&&usart1_rebuf[1]==0xFE&&usart1_rebuf[39]==0xFD)   //�ж������Ƿ���Ч
			 {
				 memcpy(PC_to_MCU,usart1_rebuf,40);
			 }
           }	
}
float main_to_motorf[10]={0xaf,0x64,0x64,0x64,0x64,0x00,0x00,0x00,0x00,0xaf};      //���͸����������
u8 main_to_motoru8[10]={0xaf,0x64,0x64,0x64,0x64,0x00,0x00,0x00,0x00,0xaf}; 
//������ĸ����1��Ӧ�����ƣ�2��Ӧ�����ƣ�3��Ӧ���ƣ�4��Ӧ�Ҵ��ƣ�5,6��Ч���ӻ�����β�������ͷ����
//������������1��2,��Ӧ�������ƣ�34��Ӧǰ����ƣ�56��Ӧ���Ҵ���
/*
���ܣ�������λ���������ĵ������
������void
����ֵ��void
*/
extern u8 speed_rata;
void motor_data_conduct(void)
{
	    u8 i=0;
//ǰ��ת��	


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


				
			
//�ϸ���Ǳ			
				main_to_motorf[3]=(float)PC_to_MCU[6]-(100-(float)PC_to_MCU[6])*((float)100-speed_rata)/100;
				main_to_motorf[4]=200-(float)PC_to_MCU[6]-(100-(float)PC_to_MCU[6])*((float)100-speed_rata)/100;
//���
				main_to_motorf[7]=(float)PC_to_MCU[7];
}
extern float real_deep;
/*
*���ܣ������ݷ��͸�PC
������void
����ֵ��void
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
	MCU_to_PC[3]=temp>>8&0xff;	  //�߰�λ
	MCU_to_PC[4]=temp&0xff;	      //�Ͱ�λ
  }
  else
  {
	MCU_to_PC[2]=0x01;	
	temp=pow(2,16)-(u16)(int16_t)(mems.roll*100);
	MCU_to_PC[3]=temp>>8&0xff;	  //�߰�λ
	MCU_to_PC[4]=temp&0xff;	      //�Ͱ�λ
  }	  
//pitch
  if(mems.pitch>0)
  {
	MCU_to_PC[5]=0x00;	
	temp=(u16)(mems.pitch*100);
	MCU_to_PC[6]=temp>>8&0xff;	  //�߰�λ
	MCU_to_PC[7]=temp&0xff;	      //�Ͱ�λ
  }
  else
  {
	MCU_to_PC[5]=0x01;	
	temp=pow(2,16)-(u16)(int16_t)(mems.pitch*100);
	MCU_to_PC[6]=temp>>8&0xff;	  //�߰�λ
	MCU_to_PC[7]=temp&0xff;	      //�Ͱ�λ
  }	
//yaw
  if(mems.yaw>0)
  {
	MCU_to_PC[8]=0x00;	
	temp=(u16)(mems.yaw*100);
	MCU_to_PC[9]=temp>>8&0xff;	  //�߰�λ
	MCU_to_PC[10]=temp&0xff;	      //�Ͱ�λ
  }
  else
  {
	MCU_to_PC[8]=0x01;	
	temp=pow(2,16)-(u16)(int16_t)(mems.yaw*100);
	MCU_to_PC[9]=temp>>8&0xff;	  //�߰�λ
	MCU_to_PC[10]=temp&0xff;	      //�Ͱ�λ
  }	
//���
	temp=abs((int)(real_deep*100));
	MCU_to_PC[11]=temp>>8&0xff;	  //�߰�λ
	MCU_to_PC[12]=temp&0xff;	      //�Ͱ�λ
	test1=temp;
//У��λ
	MCU_to_PC[21]=0xFE;       //�ӱ���ͷ��ʼ
	for(i=1;i<21;i++)
  MCU_to_PC[21]^=MCU_to_PC[i];  //�ֻ�У��
	for(i=0;i<24;i++)
	{
        while((USART1->SR&0X80)==0);
				  USART1->DR=MCU_to_PC[i];
	}
}
/*
���ܣ���b���鸴�Ƹ�aͬʱ����a���ֻ�У��ֵ
����
����ֵ
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
���ܣ���float�������u8����
����;void
����ֵ��void
*/
void float_to_u8()                    //��float�������u8
{
	u8 i=0;
	for(i=0;i<10;i++)
	  main_to_motoru8[i]=(u8)main_to_motorf[i];
}
