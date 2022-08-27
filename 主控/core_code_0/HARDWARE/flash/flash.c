#include "flash.h"
#include "delay.h"
#include "pid.h"
#include "usart.h"
u32 data_to_flash[34]={1,2,3,4,5,6,7,8,9};    //�洢��flash������
u32 data_from_flash[34];  //��flash��ȡ������
extern PD_struct deep_pid,yaw_pid;
extern u8 PC_to_MCU[40];    
/*
���ܣ���ȡָ����ַ�İ���(16λ����) 
������ faddr:����ַ 
����ֵ:��Ӧ����.
*/
u32 STMFLASH_ReadWord(u32 faddr)
{
	return *(vu32*)faddr; 
}  

//��ָ����ַ��ʼд��ָ�����ȵ�����
//�ر�ע��:��ΪSTM32F4������ʵ��̫��,û�취���ر�����������,���Ա�����
//         д��ַ�����0XFF,��ô���Ȳ������������Ҳ�������������.����
//         д��0XFF�ĵ�ַ,�����������������ݶ�ʧ.����д֮ǰȷ��������
//         û����Ҫ����,��������������Ȳ�����,Ȼ����������д. 
//�ú�����OTP����Ҳ��Ч!��������дOTP��!
//OTP�����ַ��Χ:0X1FFF7800~0X1FFF7A0F
//WriteAddr:��ʼ��ַ(�˵�ַ����Ϊ4�ı���!!),��Ϊ����������д���
//pBuffer:����ָ��
//NumToWrite:��(32λ)��(����Ҫд���32λ���ݵĸ���.stm32�Ĳ���λ��ֻ����32λ
void STMFLASH_Write(u32 WriteAddr,u32 *pBuffer,u32 NumToWrite)	
{ 
  FLASH_Status status = FLASH_COMPLETE;
	u32 addrx=0;
	u32 endaddr=0;	
  if(WriteAddr<STM32_FLASH_BASE||WriteAddr%4)return;	//�Ƿ���ַ
	FLASH_Unlock();									//���� 
  FLASH_DataCacheCmd(DISABLE);//FLASH�����ڼ�,�����ֹ���ݻ���
 		
	addrx=WriteAddr;				//д�����ʼ��ַ
	endaddr=WriteAddr+NumToWrite*4;	//д��Ľ�����ַ
	if(addrx<0X1FFF0000)			//ֻ�����洢��,����Ҫִ�в�������!!
	{
		while(addrx<endaddr)		//ɨ��һ���ϰ�.(�Է�FFFFFFFF�ĵط�,�Ȳ���)
		{
			if(STMFLASH_ReadWord(addrx)!=0XFFFFFFFF)//�з�0XFFFFFFFF�ĵط�,Ҫ�����������
			{   
				status=FLASH_EraseSector(FLASH_Sector_7,VoltageRange_3);//VCC=2.7~3.6V֮��!!
				if(status!=FLASH_COMPLETE)break;	//����������
			}else addrx+=4;
		} 
	}
	if(status==FLASH_COMPLETE)
	{
		while(WriteAddr<endaddr)//д����
		{
			if(FLASH_ProgramWord(WriteAddr,*pBuffer)!=FLASH_COMPLETE)//д������
			{ 
				break;	//д���쳣
			}
			WriteAddr+=4;
			pBuffer++;
		} 
	}
  FLASH_DataCacheCmd(ENABLE);	//FLASH��������,�������ݻ���
	FLASH_Lock();//����
} 

/*
���ܣ���ָ����ַ��ʼ����ָ�����ȵ�����
������ReadAddr:��ʼ��ַ
      pBuffer:����ָ��
      NumToRead:��(4λ)��
����ֵ��void
*/
void STMFLASH_Read(u32 ReadAddr,u32 *pBuffer,u32 NumToRead)   	
{
	u32 i;
	for(i=0;i<NumToRead;i++)
	{
		pBuffer[i]=STMFLASH_ReadWord(ReadAddr);//��ȡ4���ֽ�.
		ReadAddr+=4;//ƫ��4���ֽ�.	
	}
}
/*
����:��ȡflash�е����ݲ���������Ķ�Ӧ�Ĳ�����ȥ
����;void
����ֵ��void
*/
u8 speed_rata=100;    //Ĭ��Ϊȫ��
void refresh_from_flash(void)
{
	STMFLASH_Read(FLASH_SAVE_ADDR,data_from_flash,34);
	//���PIDֵ�Ķ�ȡ
	deep_pid.P=(u8)data_from_flash[0];
	deep_pid.P_limit=(u8)data_from_flash[1];
	deep_pid.I=(u8)data_from_flash[2];
	deep_pid.I_limit=(u8)data_from_flash[3];
	deep_pid.D=(u8)data_from_flash[4];
	deep_pid.D_limit=(u8)data_from_flash[5];
	//����PIDֵ�Ķ�ȡ
	yaw_pid.P=(u8)data_from_flash[6];
	yaw_pid.P_limit=(u8)data_from_flash[7];
	yaw_pid.I=(u8)data_from_flash[8];
	yaw_pid.I_limit=(u8)data_from_flash[9];
	yaw_pid.D=(u8)data_from_flash[10];
	yaw_pid.D_limit=(u8)data_from_flash[11];
  speed_rata=(u8)data_from_flash[28];
}
/*
���ܣ���flash�Ĳ����Ĵ���
������void
����ֵ��void
*/

void flash_data_conduct(void)                              
{
	int i=0;
	for(i=0;i<34;i++)
   data_to_flash[i]=(u32)PC_to_MCU[i+3];
	__disable_irq() ; //�ر����ж�
   STMFLASH_Write(FLASH_SAVE_ADDR,data_to_flash,34);	       //������д��flash����
	__enable_irq() ; //�����ж�
	//�������PIDֵ
	deep_pid.P=(u8)data_to_flash[0];
	deep_pid.P_limit=(u8)data_to_flash[1];
	deep_pid.I=(u8)data_to_flash[2];
	deep_pid.I_limit=(u8)data_to_flash[3];
	deep_pid.D=(u8)data_to_flash[4];
	deep_pid.D_limit=(u8)data_to_flash[5];
	//���º���PIDֵ
	yaw_pid.P=(u8)data_to_flash[6];
	yaw_pid.P_limit=(u8)data_to_flash[7];
	yaw_pid.I=(u8)data_to_flash[8];
	yaw_pid.I_limit=(u8)data_to_flash[9];
	yaw_pid.D=(u8)data_to_flash[10];
	yaw_pid.D_limit=(u8)data_to_flash[11];
	speed_rata=(u8)data_to_flash[28];
}


