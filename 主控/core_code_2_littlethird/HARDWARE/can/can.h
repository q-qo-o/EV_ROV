#ifndef __CAN_H
#define __CAN_H
#include "sys.h"

#define main_to_armm  0xE2
#define mian_to_motor 0xE1
							    							 							 				    
u8 CAN1_Mode_Init(u8 tsjw,u8 tbs2,u8 tbs1,u16 brp,u8 mode);//CAN初始化
 
u8 CAN1_Send_Msg_to_mcu(u8* msg,u8 len);						//发送数据
extern u8 CAN1_Send_Msg(u8* msg,u8 len,u16 ID);	
u8 CAN1_Receive_Msg(u8 *buf);							//接收数据
#endif






