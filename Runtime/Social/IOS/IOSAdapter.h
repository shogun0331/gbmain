

@interface IOSAdapter : NSObject
{
}



+ (void)SendMessage:(int)type;
+ (void)SendMessageWithInt:(int)type data:(int) data;
+ (void)SendMessageWithString:(int)type data:(const char*)data;
+ (void)SendMessageWithByteArray:(int)type data:(unsigned char*)data length:(int)length;

+ (void)GameStart:(const char*)data;

// multiplay interface
/**
 * 내 정보 데이터가 없는 경우 정보를 요청 하는 함수
 */
+ (void)needMyInfo;

// ====== toast message ======
+ (void)showToast:(int) type;

// ====== ingame handling ======
+ (void) gameStart:(bool)isMyFirst myData:(unsigned char*) myData myDataLength:(int)myDataLength enemyData:(unsigned char*) enemyData enemyDataLength:(int)enemyDataLength;

// ====== multiplay control ======
+ (void) pingDisconnectTime:(int) time;
+ (void)giveup;
+ (void)gameClear;
+ (void)gameFailed;
+ (void)readyForStart;

// ====== realtime message listener ======
+ (void) onRealTimeReceieveWithData:(unsigned char*) data length:(int)length;

// ====== room update listener ======
+ (void) onLeftRoom;
+ (void) onAiMatch;
+ (void) sendConnectSuccess;


+ (void) sendConnectCheck;
+ (void) pingTime;

+(void) onEnemtLeftRoom;
+(void) onGameClear;

+ (void) cross_toast:(const char*) message;
+ (NSString*) cross_getDeviceCountry;
+ (NSString*) cross_getDeviceLanguage;


@end


inline void int2char(int data, unsigned char* dest)
{
    dest[0] = (data >> 24) & 0xFF;
    dest[1] = (data >> 16) & 0xFF;
    dest[2] = (data >> 8) & 0xFF;
    dest[3] = data & 0xFF;
}

inline void int2char(int data, char* dest)
{
    dest[0] = (data >> 24) & 0xFF;
    dest[1] = (data >> 16) & 0xFF;
    dest[2] = (data >> 8) & 0xFF;
    dest[3] = data & 0xFF;
}

inline int char2int(unsigned char *data)
{
    return ((data[0] & 0xFF) << 24) | ((data[1] & 0xFF) << 16) | ((data[2] & 0xFF) << 8) | (data[3] & 0xFF);
}



enum GAMESERVICES_MESSAGE
{
    SYNCHRO = 0,
	SIGNIN,
	PING,
	MULTIPLAY,
	READY_FOR_START,
	DATA,
	ENEMY_JOIN,

	CONNECT_CHECK,
	CONNECT_SUCCESS,

	AI_MATCHING,
	ENEMY_LEFT,
	ENEMY_MISSING,
	LEFT_ROOM,
	SAVE_SAVEDGAME,
    LOAD_SAVEDGAME,

	SEND_MYINFO,
    
	END_OF_ENUM
};



