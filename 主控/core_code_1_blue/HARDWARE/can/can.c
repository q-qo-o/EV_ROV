#include "can.h"
#include "stm32f4xx.h"

 
u8 CAN1_Mode_Init(u8 tsjw,u8 tbs2,u8 tbs1,u16 brp,u8 mode)
{

  	GPIO_InitTypeDef GPIO_InitStructure; 
	CAN_InitTypeDef        CAN_InitStructure;
//  	CAN_FilterInitTypeDef  CAN_FilterInitStructure;
    //ʹ�����ʱ��
	 RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOD, ENABLE);//ʹ��PORTAʱ��	                   											 
  	 RCC_APB1PeriphClockCmd(RCC_APB1Periph_CAN1, ENABLE);//ʹ��CAN1ʱ��	
    //��ʼ��GPIO
	 GPIO_InitStructure.GPIO_Pin = GPIO_Pin_0| GPIO_Pin_1;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;//���ù���
    GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//�������
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;//100MHz
    GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//����
    GPIO_Init(GPIOD, &GPIO_InitStructure);//��ʼ��PA11,PA12
	  //���Ÿ���ӳ������
	  GPIO_PinAFConfig(GPIOD,GPIO_PinSource0,GPIO_AF_CAN1); //GPIOA11����ΪCAN1
	  GPIO_PinAFConfig(GPIOD,GPIO_PinSource1,GPIO_AF_CAN1); //GPIOA12����ΪCAN1
  	//CAN��Ԫ����
   	CAN_InitStructure.CAN_TTCM=DISABLE;	//��ʱ�䴥��ͨ��ģʽ   
  	CAN_InitStructure.CAN_ABOM=DISABLE;	//����Զ����߹���	  
  	CAN_InitStructure.CAN_AWUM=DISABLE;//˯��ģʽͨ���������(���CAN->MCR��SLEEPλ)
  	CAN_InitStructure.CAN_NART=ENABLE;	//��ֹ�����Զ����� 
  	CAN_InitStructure.CAN_RFLM=DISABLE;	//���Ĳ�����,�µĸ��Ǿɵ�  
  	CAN_InitStructure.CAN_TXFP=DISABLE;	//���ȼ��ɱ��ı�ʶ������ 
  	CAN_InitStructure.CAN_Mode= mode;	 //ģʽ���� 
  	CAN_InitStructure.CAN_SJW=tsjw;	//����ͬ����Ծ���(Tsjw)Ϊtsjw+1��ʱ�䵥λ CAN_SJW_1tq~CAN_SJW_4tq
  	CAN_InitStructure.CAN_BS1=tbs1; //Tbs1��ΧCAN_BS1_1tq ~CAN_BS1_16tq
  	CAN_InitStructure.CAN_BS2=tbs2;//Tbs2��ΧCAN_BS2_1tq ~	CAN_BS2_8tq
  	CAN_InitStructure.CAN_Prescaler=brp;  //��Ƶϵ��(Fdiv)Ϊbrp+1	
  	CAN_Init(CAN1, &CAN_InitStructure);   // ��ʼ��CAN1  
//		//���ù�����
// 	  CAN_FilterInitStructure.CAN_FilterNumber=0;	  //������0
//  	CAN_FilterInitStructure.CAN_FilterMode=CAN_FilterMode_IdMask; 
//  	CAN_FilterInitStructure.CAN_FilterScale=CAN_FilterScale_32bit; //32λ 
//  	CAN_FilterInitStructure.CAN_FilterIdHigh=0x0000;////32λID
//  	CAN_FilterInitStructure.CAN_FilterIdLow=0x0000;
//  	CAN_FilterInitStructure.CAN_FilterMaskIdHigh=0x0000;//32λMASK
//  	CAN_FilterInitStructure.CAN_FilterMaskIdLow=0x0000;
//   	CAN_FilterInitStructure.CAN_FilterFIFOAssignment=CAN_Filter_FIFO0;//������0������FIFO0
//  	CAN_FilterInitStructure.CAN_FilterActivation=ENABLE; //���������0
//  	CAN_FilterInit(&CAN_FilterInitStructure);//�˲�����ʼ��
	  return 0;
}   






//can�ڽ������ݲ�ѯ
//buf:���ݻ�����;	 
//����ֵ:0,�����ݱ��յ�;
//		 ����,���յ����ݳ���;
//u8 CAN1_Receive_Msg(u8 *buf)
//{		   		   
// 	  u32 i;
//	  u32 flag; 
//	  CanRxMsg RxMessage;
//    if( CAN_MessagePending(CAN1,CAN_FIFO0)==0)return 0;		//û�н��յ�����,ֱ���˳� 
//    CAN_Receive(CAN1, CAN_FIFO0, &RxMessage);//��ȡ����	
//	 for(i=0;i<RxMessage.DLC;i++)
//    buf[i]=RxMessage.Data[i];  
//		flag=RxMessage.StdId ;
//	  return flag;	
//}

//u32 CAN1_Receive_Msg_FIFO0(u8 *buf)
//{		   		   
// 	  u32 i;
//	  u32 flag; 
//	  CanRxMsg RxMessage;
//    if( CAN_MessagePending(CAN1,CAN_FIFO0)==0)
//			return 0;		//û�н��յ�����,ֱ���˳� 
//    CAN_Receive(CAN1, CAN_FIFO0, &RxMessage);//��ȡ����	
//	 for(i=0;i<RxMessage.DLC;i++)
//    buf[i]=RxMessage.Data[i];  
//		flag=RxMessage.StdId ;
//	  return flag;	
//}


//can����һ������(�̶���ʽ:IDΪ0X12,��׼֡,����֡)	
//len:���ݳ���(���Ϊ8)				     
//msg:����ָ��,���Ϊ8���ֽ�.
//����ֵ:0,�ɹ�;
//		 ����,ʧ��;
u8 CAN1_Send_Msg(u8* msg,u8 len,u16 ID)
{	
  u8 mbox;
  u16 i=0;
  CanTxMsg TxMessage;
  TxMessage.StdId=ID;
	TxMessage.IDE=CAN_ID_STD;
	TxMessage.RTR=CAN_RTR_Data;
  TxMessage.DLC=len;							 // ������֡��Ϣ
  for(i=0;i<len;i++)
  TxMessage.Data[i]=msg[i];				 // ��һ֡��Ϣ          
  mbox= CAN_Transmit(CAN1, &TxMessage);   
  i=0;
  while((CAN_TransmitStatus(CAN1, mbox)==CAN_TxStatus_Failed)&&(i<0XFFF))i++;	//�ȴ����ͽ���
  if(i>=0XFFF)
	return 1;
  return 0;		

}


	 
  	
	 

