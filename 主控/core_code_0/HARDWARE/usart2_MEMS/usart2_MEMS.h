#ifndef  __MEMS_H
#define  __MEMS_H
#include "sys.h"
void usart2_init(void);         //����2�ĳ�ʼ��
void MEMS_DATA_conduct(void);   //MEMS�����ݴ���
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
