//
//  MultiPlayHelper.h
//  Mine
//
//  Created by soobin on 2014. 12. 17..
//
//

#ifndef Mine_MultiPlayHelper_h
#define Mine_MultiPlayHelper_h


#define MESSAGE_LEN     32

void mpm_init();

int createMapLevel();
int getGameMode();

void checkGameStart();
int checkUserLevel();
void sendMyInfo(unsigned char* myData, int length);
void sendMyInfoWithPush(unsigned char* myData, int length);

void mpm_OppMatchConnected(const char* name);
void mpm_OppMatchDisconnected();

void mpm_dataRecv(unsigned char* p_pData, int p_nLen);

void mpm_QuickStart(unsigned char* myData, int length);
void mpm_MatchCanceled();
void mpm_MatchError();
void mpm_InviteAccept();
void mpm_networkDisconnected();


void mpm_sendReady();
void mpm_giveup();
void mpm_sendGameover();
void mpm_sendGameclear();
void mpm_rematch(unsigned char* byteArray, int length);
void mpm_sendData(unsigned char* byteArray, int length);
void mpm_leftGame();

void mpm_sendPing();
void setMatchInfo(int netVer, int mode, int level, int win, int lose);

#endif
