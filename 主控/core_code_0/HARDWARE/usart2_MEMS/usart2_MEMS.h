#ifndef  __MEMS_H
#define  __MEMS_H
#include "sys.h"
void usart2_init(void);         //串口2的初始化
void MEMS_DATA_conduct(void);   //MEMS的数据处理
typedef struct MEM
{
	float roll;
	float pitch;
	float yaw;
	float roll_rate;
	float pitch_rate;
	float yaw_rate;
	float x_acc;
	float y_acc;
	float z_acc;
	
}MEMS_struct;
#endif
