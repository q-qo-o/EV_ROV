#include "pid.h"
#include "math.h"

PD_struct deep_pid;
PD_struct yaw_pid;
//δ������޷�
/*
���ܣ����PD����
������int target_deep   Ŀ�����
      int real_deep     ʵ�����
����ֵ:��PD�����
*/
float deep_PD(float target_deep,float real_deep)
{
	deep_pid.P=100;
	deep_pid.D=30;
	deep_pid.err=target_deep-real_deep;
	deep_pid.PD_out=deep_pid.P*deep_pid.err+deep_pid.D*(deep_pid.err-deep_pid.err_last);
	deep_pid.err_last=deep_pid.err;
	return deep_pid.PD_out;
}
/*
���ܣ�����PD����
������int target_yaw  Ŀ�꺽��
      int real_yaw    ʵ�ʺ���
����ֵ������PD�����
*/

float yaw_PD(float target_yaw,float real_yaw)       //����PD����
{
	yaw_pid.P=1;
	yaw_pid.D=1;
//���Ժ����180���������
	if(target_yaw-real_yaw>300)
		yaw_pid.err=target_yaw-real_yaw-360;
	else if(target_yaw-real_yaw<-300)
		yaw_pid.err=target_yaw-real_yaw+360;	
//�������
	else
  yaw_pid.err=target_yaw-real_yaw;    
  yaw_pid.PD_out=yaw_pid.P*yaw_pid.err+yaw_pid.D*(yaw_pid.err-yaw_pid.err_last);
	yaw_pid.err_last=yaw_pid.err;
	return yaw_pid.PD_out;
}


