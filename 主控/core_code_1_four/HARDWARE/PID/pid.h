#ifndef __PID_H
#define __PID_H 	
#include "sys.h"

typedef struct
{
	float err;
	float err_last;
	float P;
	float I;
	float D;
	float PD_out;
	float P_limit;
	float I_limit;
	float D_limit;
} PD_struct;
float deep_PD(float target_deep,float real_deep);      //深度PD控制
float yaw_PD(float target_yaw,float real_yaw);       //航向PD控制


#endif


