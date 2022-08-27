#include "usart2_MEMS.h"
#include "stm32f4xx.h"                  // Device header
u8 mems_rebuf[41];    //����mems����
u8 USART2_RX_STA;     //״̬��־λ
float MEMS_data[9];   //
/*
���ܣ�MEMs���ڵĳ�ʼ��
������void
����ֵ��void
*/
void usart2_init(void)
{
    //GPIO�˿�����
	GPIO_InitTypeDef GPIO_InitStructure;
	USART_InitTypeDef USART_InitStructure;
	NVIC_InitTypeDef NVIC_InitStructure;
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOA,ENABLE); 
	//ʹ��GPIOAʱ��
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART2,ENABLE);//ʹ��USART2ʱ�� 
 //����1��Ӧ���Ÿ���ӳ��
	GPIO_PinAFConfig(GPIOA,GPIO_PinSource2,GPIO_AF_USART2); //GPIOA2����Ϊ	USART2
	GPIO_PinAFConfig(GPIOA,GPIO_PinSource3,GPIO_AF_USART2); //GPIOA3����ΪUSART2
	//USART2�˿�����
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_2 | GPIO_Pin_3; //GPIOA2��GPIOA3
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;//���ù���
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;//�ٶ�50MHz
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP; //���츴�����
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP; //����
	GPIO_Init(GPIOA,&GPIO_InitStructure); //��ʼ��PA2��PA3
   //USART2 ��ʼ������
	USART_InitStructure.USART_BaudRate = 115200;//����������
	USART_InitStructure.USART_WordLength = USART_WordLength_8b;//�ֳ�Ϊ8λ���ݸ�ʽ
	USART_InitStructure.USART_StopBits = USART_StopBits_1;//һ��ֹͣλ
	USART_InitStructure.USART_Parity = USART_Parity_No;//����żУ��λ
	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;//��Ӳ������������
	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;//�շ�ģʽ
	USART_Init(USART2, &USART_InitStructure); //��ʼ������2
	
	
	NVIC_InitStructure.NVIC_IRQChannel = USART2_IRQn;//����2�ж�ͨ��
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0;//��ռ���ȼ�3 ��ֵԽС���ȼ�Խ��
	NVIC_InitStructure.NVIC_IRQChannelSubPriority =3;//�����ȼ�3,��Ӧ���ȼ�
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;//IRQͨ��ʹ��
	NVIC_Init(&NVIC_InitStructure);//����ָ���Ĳ�����ʼ��VIC�Ĵ�����
	USART_ITConfig(USART2, USART_IT_RXNE, ENABLE);//���������ж�
	USART_ITConfig(USART2, USART_IT_IDLE, ENABLE);//�������ڽ����ж�(֡�ж�)
	USART_Cmd(USART2, ENABLE);  //ʹ�ܴ���2
}
//����2֡�ж�
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
								   //��ȡ��̬����
	              USART2_RX_STA=0;
           }	
}
/*
���ܣ�����MEMS������
������void
����ֵ��void
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
