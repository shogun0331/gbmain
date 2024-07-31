#include "IOSAdapter.h"
#include "MultiPlayHelper.h"
#include "GameCenterHelper.h"

#include <string>
#define GAMEOBJ_NAME  "GameCenterService"

// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

// Helper method to create C string copy
char* MakeStrCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

@implementation IOSAdapter

+ (void)SendMessage:(int)type
{
    char message[20] = {0x00, };
    sprintf(message, "%d", type);
    UnitySendMessage(GAMEOBJ_NAME, "receive", MakeStrCopy(message));
}

+ (void)SendMessageWithInt:(int)type data:(int)data
{
    char message[32] = {0x00, };
    sprintf(message, "%d|%d", type, data);
    UnitySendMessage(GAMEOBJ_NAME, "receiveInt", MakeStrCopy(message));
}

+ (void)SendMessageWithString:(int)type data:(const char *)data
{
    char messageType[8] = {0x00, };
    sprintf(messageType, "%d|", type);
    std::string message = messageType;
    message.append(data);
    UnitySendMessage(GAMEOBJ_NAME, "receiveString", MakeStrCopy(message.c_str()));
}

+ (void)SendMessageWithByteArray:(unsigned char*)data length:(int)length
{
    NSString* datas = @"";
    
    for(int i=0; i<length; ++i)
    {
        if(i == 0)  datas = [datas stringByAppendingFormat:@"%d", data[i]];
        else    datas = [datas stringByAppendingFormat:@"|%d", data[i]];
    }
    
    UnitySendMessage(GAMEOBJ_NAME, "receiveByteArray", MakeStrCopy([datas UTF8String]));
}

+ (void)GameStart:(const char*)data
{
    
    UnitySendMessage(GAMEOBJ_NAME, "gamestart", MakeStrCopy(data));
    
}

+ (void)needMyInfo
{
    
    [IOSAdapter SendMessage:(int)GAMESERVICES_MESSAGE::SEND_MYINFO];
    
}

// ====== ingame handling ======


+ (void) gameStart:(bool)isMyFirst myData:(unsigned char*) myData myDataLength:(int)myDataLength enemyData:(unsigned char*) enemyData enemyDataLength:(int)enemyDataLength
{
    NSString* data = [NSString stringWithFormat:@"%d", isMyFirst ? 1 : 0];
    
    for(int i=0; i<myDataLength; ++i)
    {
        if(i == 0)  data = [data stringByAppendingFormat:@"|%d", myData[i]];
        else    data = [data stringByAppendingFormat:@",%d", myData[i]];
    }
    
    for(int i=0; i<enemyDataLength; ++i)
    {
        if(i==0)    data = [data stringByAppendingFormat:@"|%d", enemyData[i]];
        else    data = [data stringByAppendingFormat:@",%d", enemyData[i]];
    }
    
    [IOSAdapter GameStart:[data UTF8String]];
}

// ====== multiplay control ======
+ (void) pingDisconnectTime:(int) time
{
    [IOSAdapter SendMessageWithInt:(int)GAMESERVICES_MESSAGE::PING data:time];
    
    if(time > 30)
    {
//        mpm_leftGame();
//        [IOSAdapter SendMessage:GAMESERVICES_MESSAGE::ENEMY_MISSING];
    }
}

+ (void) pingTime
{
   [IOSAdapter SendMessage:GAMESERVICES_MESSAGE::PING];
}


// ====== realtime message listener ======
+ (void) onRealTimeReceieveWithData:(unsigned char*) data length:(int)length
{
    [IOSAdapter SendMessageWithByteArray:data length:length];
}

// ====== room update listener ======
+ (void) onLeftRoom
{
    [IOSAdapter SendMessage:(int)GAMESERVICES_MESSAGE::LEFT_ROOM];
}

+ (void) onAiMatch
{
    [IOSAdapter SendMessage:(int)GAMESERVICES_MESSAGE::ENEMY_LEFT];
}

+ (void) sendConnectSuccess
{
    [IOSAdapter SendMessage:(int)GAMESERVICES_MESSAGE::CONNECT_SUCCESS];
}

+ (void) sendConnectCheck
{
    [IOSAdapter SendMessage:(int)GAMESERVICES_MESSAGE::CONNECT_CHECK];
}

+(void) onEnemtLeftRoom
{
    [IOSAdapter SendMessage:(int)GAMESERVICES_MESSAGE::ENEMY_LEFT];
}

+(void) onGameClear
{
    //[IOSAdapter SendMessage:(int)GAMESERVICES_MESSAGE::GAMECLEAR];
}

@end



extern "C"
{
    
    void _send(int type)
    {
    
        
        switch(type)
        {
            case GAMESERVICES_MESSAGE::SIGNIN:
                [[GameCenterHelper SharedInstance] initializeSignIn];
                break;
                
            case GAMESERVICES_MESSAGE::LEFT_ROOM:
                mpm_leftGame();
                break;
                
            case GAMESERVICES_MESSAGE::ENEMY_LEFT:
                mpm_leftGame();
                break;
                
            case GAMESERVICES_MESSAGE::LOAD_SAVEDGAME:
                [[GameCenterHelper SharedInstance] loadGame];
                break;
        }
    }

    
    void _sendWithInt(int type, int data)
    {
        
	   switch(type)
        {
                case GAMESERVICES_MESSAGE::MULTIPLAY:
                [[GameCenterHelper SharedInstance] startQuickMatchGame:data];
                break;
	    }
    }
    
    void _sendWithString(int type, const char* data)
    {
        switch(type)
        {
        }
    }
    
    void _sendWithStringInt(int type, const char* strData, int intData)
    {
        	
    }
    
    void _sendWithByteArray(int type, unsigned char* byteArray, int length)
    {
        switch(type)
        {
		   case GAMESERVICES_MESSAGE::MULTIPLAY:
                mpm_QuickStart(byteArray, length);
                break;


            case GAMESERVICES_MESSAGE::DATA:
                mpm_sendData(byteArray, length);
                break;

            case GAMESERVICES_MESSAGE::SEND_MYINFO:
                sendMyInfoWithPush(byteArray, length);
                break;
                
                
            case GAMESERVICES_MESSAGE::SAVE_SAVEDGAME:
                NSData * data = [NSData dataWithBytes:byteArray length:length];
                [[GameCenterHelper SharedInstance] saveGame:data];

                
//                        NSString* dataString = [NSString stringWithCString:data encoding:NSUTF8StringEncoding];
//                        NSData* data = [dataString dataUsingEncoding:NSUTF8StringEncoding];
                        break;
                
                
 
        }
    }
    
    void _sendWithIntByteArray(int type, int intData, unsigned char* byteArray, int length)
    {
        
    }
    
    void _sendWithString3(int type, const char* strData0, const char* strData1, const char* strData2)
    {
        
    }
    
    
}
