#include "sys.h"
#include "delay.h"
#include "usart.h"
#include "flash.h"
#include "usart6.h"
#include "string.h"
#include "ms5805.h"
#include "myiic.h"
#include "stm32f4xx.h"
#include "pid.h"
#include "usart2_MEMS.h"
#include "tim.h"

u8 usart6_send[10]={0xaf,0x64,0x64,0x64,0x64,0x00,0x00,0x00,0x00,0xaf};
extern float data_to_flash[12];
float  flash_read[12]={0};
extern u8  usart1_rebuf[40]; 
extern u8 PC_to_MCU[40];
extern float main_to_motorf[10];
extern u8 main_to_motoru8[10];
extern float real_deep;
extern MEMS_struct mems;
extern u8 code_flag;
extern u8 time_count;
mystruct deep,yaw;
float test3;
int main(void)
{ 
 
	int i=0,n=1;                                                     //ͨ����n��ֵ��ȡ��ͬ���ε�����
	delay_init(168);		//��ʱ��ʼ�� 
	IIC_Init();
	MS5837_PROM_Read();
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_2);                  //����ϵͳ�ж����ȼ�����2
	usart6_init();
  uart_DMA_init(115200);	                                         //���ڳ�ʼ��������Ϊ115200
  usart2_init();
	//��̬���ڵĳ�ʼ��
  TIM3_Int_Init(49999,83);                                         //50ms
  refresh_from_flash();                                         //�����������ȸ���PID��ֵ
//				   flash_data_conduct(); 
	while(1)
	{ 
    deep_out();           //��ȡ�������
		MEMS_DATA_conduct();  //��̬���ݵĴ���
		motor_data_conduct();
//50msִ��һ��
		if(code_flag==1)
		{
		switch(PC_to_MCU[2])
		{
 //����ģʽ
			case 0x00:         
       float_to_u8();
       memcpy(usart6_send,main_to_motoru8,10);
			   break;
 //���ģʽ					
		  case 0x02:      
			if(PC_to_MCU[6]==0x64)      //�����λ��Ҫ���д�ֱ�˶�����֤Ŀ��ֵ����ʵ��ֵ����Ȳ�������
					deep.real=real_deep;
				else
				{
					deep.real=real_deep;
					deep.target=real_deep;  //
				}
			  main_to_motorf[3]-=deep_PD(deep.target,deep.real);
				main_to_motorf[4]-=deep_PD(deep.target,deep.real);		
			  float_to_u8();
        memcpy(usart6_send,main_to_motoru8,10);				 
			   break;
//��̬�����ģʽ
      case 0x01:	
			//���
         if(PC_to_MCU[6]==0x64)      //�����λ��Ҫ���д�ֱ�˶�����֤Ŀ��ֵ����ʵ��ֵ����Ȳ�������
					deep.real=real_deep;
				else
				{
					deep.real=real_deep;
					deep.target=real_deep;  //
				}
			  main_to_motorf[3]-=deep_PD(deep.target,deep.real);
				main_to_motorf[4]-=deep_PD(deep.target,deep.real);		
			  float_to_u8();
        memcpy(usart6_send,main_to_motoru8,10);				
				//����
				if(PC_to_MCU[4]==0x64)    
					yaw.real=mems.yaw;        //��仰��֤���ƶ���ʱ����Ըı�Ŀ��ֵ
				else
				{
					yaw.real=mems.yaw;
					yaw.target=mems.yaw;  //
				}
			  main_to_motorf[1]+=yaw_PD(yaw.target,yaw.real);
				main_to_motorf[2]-=yaw_PD(yaw.target,yaw.real);
				test3=yaw_PD(yaw.target,yaw.real);;
			  float_to_u8();
        memcpy(usart6_send,main_to_motoru8,10);
				break;
			case 0x03:
       flash_data_conduct();
				 break;
			default:__nop();
		}
			usart6_send_data(usart6_send,10);
	   	code_flag=0;
	}
//1s�Ϸ�һ��
	  if(time_count>10)
		{
		 time_count=0;
	    send_to_PC();
		}
	}
}	
		
