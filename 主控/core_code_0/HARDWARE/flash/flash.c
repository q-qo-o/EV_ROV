#include "flash.h"
#include "delay.h"
#include "pid.h"
#include "usart.h"
u32 data_to_flash[34]={1,2,3,4,5,6,7,8,9};    //存储给flash的数组
u32 data_from_flash[34];  //从flash读取的数据
extern PD_struct deep_pid,yaw_pid;
extern u8 PC_to_MCU[40];    
/*
功能：读取指定地址的半字(16位数据) 
参数： faddr:读地址 
返回值:对应数据.
*/
u32 STMFLASH_ReadWord(u32 faddr)
{
	return *(vu32*)faddr; 
}  

//从指定地址开始写入指定长度的数据
//特别注意:因为STM32F4的扇区实在太大,没办法本地保存扇区数据,所以本函数
//         写地址如果非0XFF,那么会先擦除整个扇区且不保存扇区数据.所以
//         写非0XFF的地址,将导致整个扇区数据丢失.建议写之前确保扇区里
//         没有重要数据,最好是整个扇区先擦除了,然后慢慢往后写. 
//该函数对OTP区域也有效!可以用来写OTP区!
//OTP区域地址范围:0X1FFF7800~0X1FFF7A0F
//WriteAddr:起始地址(此地址必须为4的倍数!!),因为我们是以字写入的
//pBuffer:数据指针
//NumToWrite:字(32位)数(就是要写入的32位数据的个数.stm32的并行位数只能是32位
void STMFLASH_Write(u32 WriteAddr,u32 *pBuffer,u32 NumToWrite)	
{ 
  FLASH_Status status = FLASH_COMPLETE;
	u32 addrx=0;
	u32 endaddr=0;	
  if(WriteAddr<STM32_FLASH_BASE||WriteAddr%4)return;	//非法地址
	FLASH_Unlock();									//解锁 
  FLASH_DataCacheCmd(DISABLE);//FLASH擦除期间,必须禁止数据缓存
 		
	addrx=WriteAddr;				//写入的起始地址
	endaddr=WriteAddr+NumToWrite*4;	//写入的结束地址
	if(addrx<0X1FFF0000)			//只有主存储区,才需要执行擦除操作!!
	{
		while(addrx<endaddr)		//扫清一切障碍.(对非FFFFFFFF的地方,先擦除)
		{
			if(STMFLASH_ReadWord(addrx)!=0XFFFFFFFF)//有非0XFFFFFFFF的地方,要擦除这个扇区
			{   
				status=FLASH_EraseSector(FLASH_Sector_7,VoltageRange_3);//VCC=2.7~3.6V之间!!
				if(status!=FLASH_COMPLETE)break;	//发生错误了
			}else addrx+=4;
		} 
	}
	if(status==FLASH_COMPLETE)
	{
		while(WriteAddr<endaddr)//写数据
		{
			if(FLASH_ProgramWord(WriteAddr,*pBuffer)!=FLASH_COMPLETE)//写入数据
			{ 
				break;	//写入异常
			}
			WriteAddr+=4;
			pBuffer++;
		} 
	}
  FLASH_DataCacheCmd(ENABLE);	//FLASH擦除结束,开启数据缓存
	FLASH_Lock();//上锁
} 

/*
功能：从指定地址开始读出指定长度的数据
参数：ReadAddr:起始地址
      pBuffer:数据指针
      NumToRead:字(4位)数
返回值：void
*/
void STMFLASH_Read(u32 ReadAddr,u32 *pBuffer,u32 NumToRead)   	
{
	u32 i;
	for(i=0;i<NumToRead;i++)
	{
		pBuffer[i]=STMFLASH_ReadWord(ReadAddr);//读取4个字节.
		ReadAddr+=4;//偏移4个字节.	
	}
}
/*
功能:读取flash中的数据并把它分配的对应的参数中去
参数;void
返回值：void
*/
u8 speed_rata=100;    //默认为全速
void refresh_from_flash(void)
{
	STMFLASH_Read(FLASH_SAVE_ADDR,data_from_flash,34);
	//深度PID值的读取
	deep_pid.P=(u8)data_from_flash[0];
	deep_pid.P_limit=(u8)data_from_flash[1];
	deep_pid.I=(u8)data_from_flash[2];
	deep_pid.I_limit=(u8)data_from_flash[3];
	deep_pid.D=(u8)data_from_flash[4];
	deep_pid.D_limit=(u8)data_from_flash[5];
	//航向PID值的读取
	yaw_pid.P=(u8)data_from_flash[6];
	yaw_pid.P_limit=(u8)data_from_flash[7];
	yaw_pid.I=(u8)data_from_flash[8];
	yaw_pid.I_limit=(u8)data_from_flash[9];
	yaw_pid.D=(u8)data_from_flash[10];
	yaw_pid.D_limit=(u8)data_from_flash[11];
  speed_rata=(u8)data_from_flash[28];
}
/*
功能：给flash的参数的处理
参数：void
返回值：void
*/

void flash_data_conduct(void)                              
{
	int i=0;
	for(i=0;i<34;i++)
   data_to_flash[i]=(u32)PC_to_MCU[i+3];
	__disable_irq() ; //关闭总中断
   STMFLASH_Write(FLASH_SAVE_ADDR,data_to_flash,34);	       //把数据写到flash里面
	__enable_irq() ; //打开总中断
	//更新深度PID值
	deep_pid.P=(u8)data_to_flash[0];
	deep_pid.P_limit=(u8)data_to_flash[1];
	deep_pid.I=(u8)data_to_flash[2];
	deep_pid.I_limit=(u8)data_to_flash[3];
	deep_pid.D=(u8)data_to_flash[4];
	deep_pid.D_limit=(u8)data_to_flash[5];
	//更新航向PID值
	yaw_pid.P=(u8)data_to_flash[6];
	yaw_pid.P_limit=(u8)data_to_flash[7];
	yaw_pid.I=(u8)data_to_flash[8];
	yaw_pid.I_limit=(u8)data_to_flash[9];
	yaw_pid.D=(u8)data_to_flash[10];
	yaw_pid.D_limit=(u8)data_to_flash[11];
	speed_rata=(u8)data_to_flash[28];
}


