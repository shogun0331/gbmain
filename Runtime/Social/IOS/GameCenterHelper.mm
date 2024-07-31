#import "GameCenterHelper.h"
#import "MultiPlayHelper.h"

#import "IOSAdapter.h"



@implementation GameCenterHelper

@synthesize mainviewController;
@synthesize myMatch;
@synthesize currentPlayerID, otherPlayerID;
@synthesize gameCenterAuthenticationComplete;



static GameCenterHelper *_instance = nil;

char g_RecvBuf[MESSAGE_LEN];

+ (GameCenterHelper *)SharedInstance
{
    @synchronized(self){
        if (_instance == nil){
            _instance = [[self alloc] init];;
        }
    }
    
    return _instance;
}

- (void)CloseSharedInstance
{
    if (_instance) [_instance release];
    _instance = nil;
}

- (void)initializeSignIn
{
    self.myMatch = nil;
    self.gameCenterAuthenticationComplete = NO;
    self.otherPlayerID = nil;
    matchStarted = NO;
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    iosVer = [[vComp objectAtIndex:0] intValue];
    
    GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
    
    /*
     The authenticateWithCompletionHandler method is like all completion handler methods and runs a block
     of code after completing its task. The difference with this method is that it does not release the
     completion handler after calling it. Whenever your application returns to the foreground after
     running in the background, Game Kit re-authenticates the user and calls the retained completion
     handler. This means the authenticateWithCompletionHandler: method only needs to be called once each
     time your application is launched. This is the reason the sample authenticates in the application
     delegate's application:didFinishLaunchingWithOptions: method instead of in the view controller's
     viewDidLoad method.
     
     Remember this call returns immediately, before the user is authenticated. This is because it uses
     Grand Central Dispatch to call the block asynchronously once authentication completes.
     */
    if (localPlayer){
        localPlayer.authenticateHandler = ^(UIViewController *viewController, NSError *error){
            if (viewController){
                [mainviewController presentViewController:viewController animated:YES completion:nil];
            }else if (localPlayer.isAuthenticated){
                self.currentPlayerID = localPlayer.playerID;
                self.gameCenterAuthenticationComplete = YES;
                
                // 로그인 설정
                [IOSAdapter SendMessage:GAMESERVICES_MESSAGE::SIGNIN];
                
                // 세이브드 게임 로드
//                [self loadGame];
              
                [self inviteRegHandlerProc : localPlayer];
                
            }else{
                self.gameCenterAuthenticationComplete = NO;
//                [IOSAdapter SendMessage:GAMESERVICES_MESSAGE::SIGNOUT];
                
            }
        };
    }
}

- (void)inviteRegHandlerProc : (GKLocalPlayer *)localPlayer
{
    [localPlayer registerListener:self];
}

- (void)startQuickMatchGame : (int)chanel
{
    self.myMatch = nil;
    matchStarted = NO;
    
    GKMatchRequest *request = [[GKMatchRequest alloc] init];
    request.minPlayers = 2;
    request.maxPlayers = 2;
    
    //request.playerAttributes = chanel;

    
    
    // 애플 UI 없이 매칭 시작
    
    [[GKMatchmaker sharedMatchmaker] findMatchForRequest:request withCompletionHandler:^(GKMatch * _Nullable match, NSError * _Nullable error) {

        if(error)
        {
            NSLog(@"error");

        }
        else if(match != nil)
        {

            self.myMatch = match;
            match.delegate = self;

        }
    }];
    
}




- (void)dataSend : (NSData*)pData
{
    if (!self.otherPlayerID || !self.myMatch || !pData)
    {
        return;
    }
    
    NSError* error = nil;
    [self.myMatch sendDataToAllPlayers: pData withDataMode: GKMatchSendDataReliable error:&error];
    if (error){
        // Handle the error.
        
       
        matchStarted = NO;
        mpm_networkDisconnected();
    }
}

- (void)dataSend_unreliable : (NSData*)pData
{
    if (!self.otherPlayerID || !self.myMatch || !pData) return;
    
    NSError* error = nil;
    [self.myMatch sendDataToAllPlayers: pData withDataMode: GKMatchSendDataUnreliable error:&error];
    if (error){
        // Handle the error.
        //NSLog(@"dataSend error");
        
        matchStarted = NO;
        mpm_networkDisconnected();
    }
}


- (void)disconnectMatch
{
    
    if (self.myMatch) [self.myMatch disconnect];
    
    self.myMatch = nil;
    self.otherPlayerID = nil;
    matchStarted = NO;
}

- (bool)isConnected
{
    if (self.myMatch) return true;
    
    return false;
}

- (void)matchingCancel {
    
   
    [[GKMatchmaker sharedMatchmaker] cancel];
}

- (void)showLeaderboard
{
    GKGameCenterViewController *gameCenterController = [[GKGameCenterViewController alloc] init];
    gameCenterController.gameCenterDelegate = self;
    gameCenterController.viewState = GKGameCenterViewControllerStateLeaderboards;
    [mainviewController presentViewController:gameCenterController animated:YES completion:nil];
    
    [gameCenterController release];
}

- (void)showAchievements
{
    GKGameCenterViewController *gameCenterController = [[GKGameCenterViewController alloc] init];
    gameCenterController.gameCenterDelegate = self;
    gameCenterController.viewState = GKGameCenterViewControllerStateAchievements;
    [mainviewController presentViewController:gameCenterController animated:YES completion:nil];
    
    [gameCenterController release];
}

- (void)submitScore : (NSString *)category score:(long)score
{
    if ([GKLocalPlayer localPlayer].isAuthenticated){
        GKScore *scoreReporter = nil;
        
        scoreReporter = [[[GKScore alloc] initWithLeaderboardIdentifier:category] autorelease];
        scoreReporter.value = score;
        scoreReporter.context = 0;
        
        NSArray *scores = @[scoreReporter];
        [GKScore reportScores:scores withCompletionHandler:^(NSError *error) {
            //Do something interesting here.
        }];
    }
}

- (void)achievement : (NSString *)pId percent:(float)percent
{
    if ([GKLocalPlayer localPlayer].isAuthenticated){
        GKAchievement* achieve = [[[GKAchievement alloc] initWithIdentifier:pId] autorelease];
        if (!achieve.completed){
            achieve.percentComplete = percent;
            
            NSArray *achievements = [NSArray arrayWithObjects:achieve, nil];
            
            [GKAchievement reportAchievements:achievements withCompletionHandler:^(NSError *error)
             {
                 if (error == nil && percent >= 100)
                 {
//                     [IOSAdapter SendMessageWithString:GAMESERVICES_MESSAGE::ACHI_UNLOCK data:[pId UTF8String]];
                 }
             }];
        }
    }
}

#pragma mark -- GKMatchmakerViewControllerDelegate
- (void)matchmakerViewControllerWasCancelled:(GKMatchmakerViewController *)viewController
{
    
    NSLog(@"USER_CANCEL");
    [mainviewController dismissViewControllerAnimated:YES completion:nil];
    mpm_MatchCanceled();
}

- (void)matchmakerViewController:(GKMatchmakerViewController *)viewController didFailWithError:(NSError *)error
{
    NSLog(@"CONNECT_FAIL");
    [mainviewController dismissViewControllerAnimated:YES completion:nil];
    mpm_MatchError();
}

// if a match has been created and everyone is ready to start, your delegate’s matchmakerViewController:didFindMatch: method is called.
- (void)matchmakerViewController:(GKMatchmakerViewController *)viewController didFindMatch:(GKMatch *)match
{
    NSLog(@"didFindMatch");
    [mainviewController dismissViewControllerAnimated:YES completion:nil];
    
    self.myMatch = match; // Use a retaining property to retain the match.
    match.delegate = self;
    
    if (!matchStarted && match.expectedPlayerCount == 0){
        matchStarted = YES;
        
        NSArray* pArray = [match players];
        GKPlayer* pPlay = nil;
        
        for (int i=0; i<pArray.count; i++){
            pPlay = pArray[i];
            if (![pPlay.playerID isEqualToString:self.currentPlayerID]){
                self.otherPlayerID = pPlay.playerID;
                mpm_OppMatchConnected([pPlay.displayName UTF8String]);
                break;
            }
        }
        
        ///////////////////////////////////
        
        // Handle initial match negotiation.
        
    }
}

#pragma mark -- GKMatchDelegate
- (void)match:(GKMatch *)match player:(GKPlayer *)player didChangeConnectionState:(GKPlayerConnectionState)state // Mobirix_20201218
{
    if(match != myMatch) return;
    
    switch (state) {
        case GKPlayerStateConnected:
            {
                NSLog(@"GKPlayerStateConnected");
                matchStarted = YES;
                self.otherPlayerID = player.playerID;
                
                // Handle initial match negotiation.
                
                NSArray* pArray = [match players];
                GKPlayer* pPlay = nil;
                
                for (int i=0; i<pArray.count; i++){
                    pPlay = pArray[i];
                   
                    if ([pPlay.playerID isEqualToString:player.playerID])
                    {
                        mpm_OppMatchConnected([pPlay.displayName UTF8String]);
                        break;
                    }
                }
                
                
            }
            break;
            
        case GKPlayerStateDisconnected:
            NSLog(@"GKPlayerStateDisconnected");
            
            // A Player just disconnected.
            matchStarted = NO;
            self.otherPlayerID = nil;   // retain property 이므로 자동 해제도 처리한다.
            
            [IOSAdapter SendMessage:GAMESERVICES_MESSAGE::ENEMY_MISSING];
            mpm_OppMatchDisconnected();
            break;
            
        default:
            break;
    }
}

- (void)match:(GKMatch *)match didReceiveData:(NSData *)data fromRemotePlayer:(GKPlayer *)player // Mobirix_20201218
{
    
    
    long length = [data length];
    Byte recvBuf[length];

    [data getBytes:recvBuf length:length];

    mpm_dataRecv(recvBuf, (int)length);
}



// Called when the match cannot connect to any other players.
- (void)match:(GKMatch *)match didFailWithError:(NSError *)error
{
     NSLog(@"didFailWithError");
    matchStarted = NO;
    mpm_networkDisconnected();
}

- (void) lookupPlayers {
    [GKPlayer loadPlayersForIdentifiers:myMatch.playerIDs withCompletionHandler:^(NSArray *players, NSError *error) {
        if (error != nil){
            matchStarted = NO;
            mpm_networkDisconnected();
        }else{
            GKPlayer* pPlay = nil;
            if (players){
                for (int i=0; i<players.count; i++){
                    pPlay = players[i];
                    if (![pPlay.playerID isEqualToString:self.currentPlayerID]){
                        self.otherPlayerID = pPlay.playerID;
                        
                        // 게임센터에서 심볼릭 유니코드를 포함해서
                        NSString* trimText = [[pPlay.displayName componentsSeparatedByCharactersInSet:[NSCharacterSet characterSetWithCharactersInString:@"\U0000200e\U0000202a\U0000202c"]] componentsJoinedByString:@""];
                        
                        const char* szOppname = [trimText UTF8String];
                        
                        mpm_OppMatchConnected(szOppname);
                        //szOppname    const char *    "\U0000200e“\U0000202a pulito1234 \U0000202c”"    0x183b35f0
                        
                        break;
                    }
                }
            }
        }
    }];
}

#pragma mark -- GKLocalPlayerListener
- (void)player:(GKPlayer *)player didAcceptInvite:(GKInvite *)invite
{
    GKMatchmakerViewController *mmvc = [[GKMatchmakerViewController alloc] initWithInvite:invite];
    mmvc.matchmakerDelegate = self;
    [mainviewController presentViewController:mmvc animated:YES completion:nil];
    
    self.myMatch = nil;
    matchStarted = NO;
    mpm_InviteAccept();
    
    [mmvc release];
}

// ios 7
- (void)player:(GKPlayer *)player didRequestMatchWithPlayers:(NSArray *)playerIDsToInvite
{
    self.myMatch = nil;
    matchStarted = NO;
    
    GKMatchRequest *request = [[GKMatchRequest alloc] init];
    request.minPlayers = 2;
    request.maxPlayers = 2;
    request.playersToInvite = playerIDsToInvite;
    
    GKMatchmakerViewController *mmvc = [[GKMatchmakerViewController alloc] initWithMatchRequest:request];
    mmvc.matchmakerDelegate = self;
    [mainviewController presentViewController:mmvc animated:YES completion:nil];
    [mmvc release];
    [request release];
}

// ios 8
- (void)player:(GKPlayer *)player didRequestMatchWithRecipients:(NSArray *)recipientPlayers
{
    self.myMatch = nil;
    matchStarted = NO;
    
    GKMatchRequest *request = [[GKMatchRequest alloc] init];
    request.minPlayers = 2;
    request.maxPlayers = 2;
    request.playersToInvite = recipientPlayers;
    
    GKMatchmakerViewController *mmvc = [[GKMatchmakerViewController alloc] initWithMatchRequest:request];
    mmvc.matchmakerDelegate = self;
    [mainviewController presentViewController:mmvc animated:YES completion:nil];
    [mmvc release];
    [request release];
}

#pragma mark -- GKGameCenterControllerDelegate
- (void)gameCenterViewControllerDidFinish:(GKGameCenterViewController *)gameCenterViewController
{
    [mainviewController dismissViewControllerAnimated:YES completion:nil];
}

#pragma mark -- GKSavedGame
- (void)saveGame:(NSData*)data
{
    GKLocalPlayer* localPlayer = [GKLocalPlayer localPlayer];
    if(!localPlayer.isAuthenticated)
    {
        return;
    }
    
    [localPlayer saveGameData:data withName:@"mxsavedgame" completionHandler:^(GKSavedGame * _Nullable savedGame, NSError * _Nullable error) {
        if(error == nil)
        {
            // snapshot successfully saved
            NSLog(@"savedSnapshot success. name %@, deviceName %@", [savedGame name], [savedGame deviceName]);
        }
        else
        {
            NSLog(@"saveSnapshot error : %@", [error localizedDescription]);
        }
    }];
}




- (void)loadGame
{
    GKLocalPlayer* localPlayer = [GKLocalPlayer localPlayer];
    if(!localPlayer.isAuthenticated)
        return;
    
    [localPlayer fetchSavedGamesWithCompletionHandler:^(NSArray<GKSavedGame *> * _Nullable savedGames, NSError * _Nullable error) {
        if(error != nil)
        {
            [IOSAdapter SendMessageWithString:GAMESERVICES_MESSAGE::LOAD_SAVEDGAME data:""];
            return;
        }
        
        int saveCount = (int)((unsigned long)[savedGames count]);
        NSUInteger i = 0;
        
        
        NSUInteger foundIndex = -1;
        
        for(i=0; i<saveCount; ++i)
        {
            //            NSString *saveName = savedGames[i].name;
            //
            //            if([saveName isEqualToString:@"mxsavedgame"])
            //            {
            //                foundIndex = i;
            //            }
            if(i == 0)
            {
                foundIndex = i;
            }
            else
            {
                if([[savedGames[foundIndex] modificationDate] compare:[savedGames[i] modificationDate]] < 0)
                {
                    foundIndex = i;
                }
            }
            
        }
        
        NSArray<GKSavedGame*>* savedGameList = savedGames;
        
        
        
        if(foundIndex != -1)
        {
            [savedGames[foundIndex] loadDataWithCompletionHandler:^(NSData * _Nullable data, NSError * _Nullable error) {
                
                NSString* myString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
                [IOSAdapter SendMessageWithString:GAMESERVICES_MESSAGE::LOAD_SAVEDGAME data:[myString UTF8String]];
                
                
                [[GKLocalPlayer localPlayer] resolveConflictingSavedGames:savedGameList withData:data completionHandler:^(NSArray<GKSavedGame *> * _Nullable savedGames, NSError * _Nullable error) {
                    if(error != nullptr)
                    {
                        //                        NSLog(@"resolve conflicting error : %@", [error localizedDescription]);
                    }
                    else
                    {
                        //                        NSLog(@"resolve conflicting success");
                    }
                    
                }];
                
            }];
        }
        else
        {
            [IOSAdapter SendMessageWithString:GAMESERVICES_MESSAGE::LOAD_SAVEDGAME data:""];
        }
    }];
}

@end
