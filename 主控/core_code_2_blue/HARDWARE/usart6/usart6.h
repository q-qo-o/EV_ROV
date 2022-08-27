#ifndef __USART6_H
#define __USART6_H
#include "sys.h"
void usart6_init(void);      //usart6初始化
void usart6_send_data(u8 *data,u8 len);     //usart6发送数据
#endif



