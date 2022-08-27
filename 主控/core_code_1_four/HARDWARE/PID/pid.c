#include "pid.h"
#include "math.h"

PD_struct deep_pid;

PD_struct yaw_pid;
//未加输出限幅
/*
功能：深度PD控制
参数：int target_deep   目标深度
      int real_deep     实际深度
返回值:度PD输出量
*/
float deep_PD(float target_deep,float real_deep)
{
	
	deep_pid.P = 50;
	deep_pid.D = 5;
	deep_pid.err = target_deep-real_deep;
	
	deep_pid.PD_out = deep_pid.P*deep_pid.err+deep_pid.D*(deep_pid.err-deep_pid.err_last);
	
	deep_pid.err_last = deep_pid.err;
	return deep_pid.PD_out;
}
/*
功能：航向PD控制
参数：int target_yaw  目标航向
      int real_yaw    实际航向
返回值：航向PD输出量
*/

float yaw_PD(float target_yaw,float real_yaw)       //航向PD控制
{
	yaw_pid.P=1;
	yaw_pid.D=1;
//争对航向±180°跳变情况
	if(target_yaw-real_yaw>300)
		yaw_pid.err=target_yaw-real_yaw-360;
	else if(target_yaw-real_yaw<-300)
		yaw_pid.err=target_yaw-real_yaw+360;	
//正常情况
	else
  yaw_pid.err=target_yaw-real_yaw;    
  yaw_pid.PD_out=yaw_pid.P*yaw_pid.err+yaw_pid.D*(yaw_pid.err-yaw_pid.err_last);
	yaw_pid.err_last=yaw_pid.err;
	return yaw_pid.PD_out;
}

