//
//  MultiPlayHelper.mm
//  Mine
//
//  Created by soobin on 2014. 12. 17..
//
//

#import "MultiPlayHelper.h"
#import "GameCenterHelper.h"

#import "IOSAdapter.h"


typedef enum
{
    THREAD_NONE = 0,
    THREAD_MAPSELECT,
    THREAD_GAME_ING
} THREAD_TYPE;

// 동기화 여부 판단하는 변수
BOOL isSendInfo;
BOOL isRecvInfo;

// 누구 맵을 사용할지 비교하는 변수
unsigned char _myUserLevel[4] = {0x00, };
unsigned char _enemyUserLevel[4] = {0x00, };

NSData* _myData = nullptr;
NSData* _enemyData = nullptr;

int _pingTime = 0;


NSMutableArray* pPacketList = nil;





// 스레드를 위해 할 수 없이....
@interface packetThread : NSObject{
    int threadType;
    NSThread *gthread;
}


@property (nonatomic) int threadType;
@property (nonatomic, retain) NSThread *gthread;

// method
+ (packetThread *)sharedInstance;

// 스레드
- (void)threadStart;
- (void)threadEnd;
- (void)threadFunc;

// 메시지
- (void)messageQueueInput : (NSData*)pSendData;

@end

@implementation packetThread

@synthesize threadType;
@synthesize gthread;

static packetThread* _mgrInstance = nil;

+ (packetThread *)sharedInstance
{
    @synchronized(self){
        if (_mgrInstance == nil){
            _mgrInstance = [[self alloc] init];
            mpm_init();
        }
    }
    
    return _mgrInstance;
}

- (id) init
{
    if ((self = [super init])){
        self.threadType = THREAD_NONE;
        self.gthread = nil;
    }
    
    return self;
}

#pragma mark - Thread
- (void)threadStart
{
    [self threadEnd];
    threadType = THREAD_MAPSELECT;
    gthread = [[NSThread alloc] initWithTarget:self selector:@selector(threadFunc) object:nil];
    [gthread start];
    _pingTime = 0;
}

- (void)threadEnd
{
    if (gthread){
        [gthread cancel];
        [gthread release];
        threadType = THREAD_NONE;
        gthread = nil;
    }
}

- (void)threadFunc
{
    
    NSData* pSendData = nil;
    
    // 내부에서 객체를 생성하면 반드시 autoreleasepool를 만들어야 한다.
    NSAutoreleasePool* pool = [[NSAutoreleasePool alloc]init];
    
    // ping 체크를 위한 변수
    int pingSendTimer = 0;
    
    while ([[NSThread currentThread] isCancelled] == NO)
    {
        [NSThread sleepForTimeInterval:0.01f];    // 0.01초 sleep
        
        @synchronized(self){
            if (pPacketList.count > 0){
                pSendData = pPacketList[0];
                [[GameCenterHelper SharedInstance] dataSend : pSendData];
                [pPacketList removeObjectAtIndex:0];
            }
        }
        
        switch (threadType) {
            case THREAD_NONE:
                break;
            case THREAD_MAPSELECT:
                if(isSendInfo && isRecvInfo)
                {
                    isSendInfo = NO;
                    isRecvInfo = NO;
                    
                    _pingTime = 0;
                    checkGameStart();
                 
                    
                }
                else
                {
                    ++_pingTime;
                    
                    if(_pingTime >= 100)
                    {
                        _pingTime = 0;
                        sendMyInfo((Byte*)[_myData bytes], (int)[_myData length]);
                    }
                }
                break;
            case THREAD_GAME_ING:
                ++_pingTime;
                
                [IOSAdapter pingDisconnectTime:_pingTime/100];
                
                // send ping every 1 seconds
                ++pingSendTimer;
                if(pingSendTimer >= 100)
                {
                    pingSendTimer = 0;
                    
                    mpm_sendPing();
                }
                break;
            default:
                threadType = THREAD_NONE;
                [gthread cancel];
                break;
        }
    }
    
    [pool release];
    
}


- (void)messageQueueInput : (NSData*)pSendData
{
    @synchronized(self){
        [pPacketList addObject:pSendData];
    }
}

@end

void checkGameStart()
{
    NSLog(@"checkGameStart");
    int result = checkUserLevel();
    
    if(result > 0)
    {   // 내가 이김
    }
    else if(result < 0)
    {
    }
    else
    {
        isSendInfo = NO;
        isRecvInfo = NO;
        sendMyInfo((Byte*)[_myData bytes], (int)[_myData length]);
        return;
    }
    
    Byte enemyData[[_enemyData length]];
    long length = [_enemyData length];
    
    memcpy(enemyData, [_enemyData bytes], [_enemyData length]);
    if(_enemyData != nullptr)
    {
        [_enemyData release];
        _enemyData = nullptr;
    }
    _enemyData = [NSData dataWithBytes:enemyData+5 length:length-5];
    [_enemyData retain];
    
    [packetThread sharedInstance].threadType = THREAD_GAME_ING;
    
    isSendInfo = NO;
    isRecvInfo = NO;
    
    [IOSAdapter gameStart:result > 0 myData:(Byte*)[_myData bytes] myDataLength:(int)[_myData length] enemyData:(Byte*)[_enemyData bytes] enemyDataLength:(int)[_enemyData length]];
    
}

int checkUserLevel()
{
    if (_myUserLevel[0] > _enemyUserLevel[0]) return 1;
    else if (_myUserLevel[0] < _enemyUserLevel[0]) return -1;
    
    if (_myUserLevel[1] > _enemyUserLevel[1]) return 1;
    else if (_myUserLevel[1] < _enemyUserLevel[1]) return -1;
    
    if (_myUserLevel[2] > _enemyUserLevel[2]) return 1;
    else if (_myUserLevel[2] < _enemyUserLevel[2]) return -1;
    
    if (_myUserLevel[3] > _enemyUserLevel[3]) return 1;
    else if (_myUserLevel[3] < _enemyUserLevel[3]) return -1;
    
    return 0;
}


void sendMyInfo(unsigned char* myData, int length)
{
    
    Byte sendData[length + 5];
    
    sendData[0] = (Byte)SYNCHRO;
    sendData[1] = _myUserLevel[0] = arc4random() % 127;
    sendData[2] = _myUserLevel[1] = arc4random() % 127;
    sendData[3] = _myUserLevel[2] = arc4random() % 127;
    sendData[4] = _myUserLevel[3] = arc4random() % 127;
    
    memcpy(sendData+5, myData, length);
    
    
    NSData* data = [NSData dataWithBytes:sendData length:sizeof(sendData)];
    [[packetThread sharedInstance] messageQueueInput:data];
    
    
    [packetThread sharedInstance].threadType = THREAD_MAPSELECT;
    isSendInfo = YES;
    
    
}
void sendMyInfoWithPush(unsigned char* myData, int length)
{
    if(_myData != nullptr)
    {
        [_myData release];
        _myData = nullptr;
    }
    _myData = [NSData dataWithBytes:myData length:length];
    [_myData retain];
    
    sendMyInfo(myData, length);
}

void mpm_OppMatchConnected(const char* name)
{
//    if(name)
//        [IOSAdapter SendMessageWithString:ENEMY_NAME data:name];
    
    sendMyInfo((Byte*)[_myData bytes], (int)[_myData length]);
}

void mpm_OppMatchDisconnected()
{
    [IOSAdapter onLeftRoom];
    
    [[packetThread sharedInstance] threadEnd];
}

void mpm_dataRecv(unsigned char* p_pData, int p_nLen)
{
    if(p_nLen <= 0)
        return;
    
    // reset ping time
    _pingTime = 0;
    
    Byte recvData[p_nLen];
    memcpy(recvData, p_pData, p_nLen);
    
    switch (recvData[0])
    {
        case SYNCHRO:
        {
            if(_enemyData != nullptr)
            {
                [_enemyData release];
                _enemyData = nullptr;
            }
            _enemyData = [NSData dataWithBytes:p_pData length:p_nLen];
            [_enemyData retain];
            
            _enemyUserLevel[0] = recvData[1];
            _enemyUserLevel[1] = recvData[2];
            _enemyUserLevel[2] = recvData[3];
            _enemyUserLevel[3] = recvData[4];
            
            isRecvInfo = YES;
            break;
        }
        case PING:
            break;
        default:
        {
            Byte realData[p_nLen-1];
            memcpy(realData, recvData+1, p_nLen-1);
            [IOSAdapter onRealTimeReceieveWithData:realData length:p_nLen-1];
            break;
        }
    }
}

void mpm_QuickStart(unsigned char* myData, int length)
{
    if(_myData != nullptr)
    {
        [_myData release];
        _myData = nullptr;
    }
    _myData = [NSData dataWithBytes:myData length:length];
    [_myData retain];
    
    
    // 스레드 시작
    [pPacketList removeAllObjects];
    
    isSendInfo = NO;
    isRecvInfo = NO;
    
    [packetThread sharedInstance].threadType = THREAD_NONE;
    [[packetThread sharedInstance] threadStart];
}

void mpm_MatchCanceled()
{
    [[packetThread sharedInstance] threadEnd];
}

void mpm_MatchError()
{
    [[packetThread sharedInstance] threadEnd];
    
    // 에러 메시지
//    NSString* nsStr = @"Network Error";
//    if (ex_cLanguage == KOR_LANG) nsStr = @"네트워크 에러";
//    else if (ex_cLanguage == JPN_LANG) nsStr = @"ネットワークエラー";
    
//    [[[[iToast makeText:nsStr] setGravity:iToastGravityCenter] setDuration:iToastDurationNormal] show];
}

void mpm_InviteAccept()
{
    // 스레드 시작
    [pPacketList removeAllObjects];
    
    isSendInfo = NO;
    isRecvInfo = NO;
    [packetThread sharedInstance].threadType = THREAD_NONE;
    [[packetThread sharedInstance] threadStart];
}

void mpm_networkDisconnected()
{
    [[packetThread sharedInstance] threadEnd];
}


void mpm_init()
{
    pPacketList = [[NSMutableArray alloc] init];
}

void mpm_rematch(unsigned char* byteArray, int length)
{
    if(_myData != nullptr)
    {
        [_myData release];
        _myData = nullptr;
    }
    _myData = [NSData dataWithBytes:byteArray length:length];
    [_myData retain];
    
    sendMyInfo((Byte*)[_myData bytes], (int)[_myData length]);
//    [IOSAdapter SendMessageWithString:MESSAGE::TOAST data:"WAIT_RESPONSE"];
    
}

void mpm_leftGame()
{
    [[packetThread sharedInstance] threadEnd];
    [[GameCenterHelper SharedInstance] disconnectMatch];
}

void mpm_sendPing()
{
    Byte datas[1] = {0x00, };
    datas[0] = (Byte)PING;
    
    NSData* data = [NSData dataWithBytes:datas length:sizeof(datas)];
    [[packetThread sharedInstance] messageQueueInput:data];
}

void mpm_sendData(unsigned char* byteArray, int length)
{
    Byte datas[length+1];
    datas[0] = (Byte)DATA;
    memcpy(datas+1, byteArray, length);
    
    NSData* data = [NSData dataWithBytes:datas length:length+1];
    [[packetThread sharedInstance] messageQueueInput:data];
}
