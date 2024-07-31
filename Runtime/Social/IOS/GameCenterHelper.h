//
//  GameCenterHelper.h
//  TheKingOfCheckers
//
//  Created by 박장호 on 2014. 11. 26..
//
//

#ifndef TheKingOfCheckers_GameCenterHelper_h
#define TheKingOfCheckers_GameCenterHelper_h


#import <GameKit/GameKit.h>

// http://www.raywenderlich.com/3276/game-center-tutorial-for-ios-how-to-make-a-simple-multiplayer-game-part-12 참고
// https://developer.apple.com/library/ios/documentation/NetworkingInternet/Conceptual/GameKit_Guide/Matchmaking/Matchmaking.html#//apple_ref/doc/uid/TP40008304-CH9-SW1 참고

@interface GameCenterHelper : NSObject<GKMatchmakerViewControllerDelegate,
                                        GKMatchDelegate,
                                        GKLocalPlayerListener,
                                        GKGameCenterControllerDelegate>{
                                           
    UIViewController* mainviewController;
    GKMatch* myMatch;
    BOOL matchStarted;
    int iosVer;
                                          
}


@property (nonatomic, retain) UIViewController *mainviewController;
@property (nonatomic, retain) GKMatch* myMatch;

// currentPlayerID is the value of the playerID last time we authenticated.
@property (retain, readwrite) NSString* currentPlayerID;
@property (retain, readwrite) NSString* otherPlayerID;

// isGameCenterAuthenticationComplete is set after authentication, and authenticateWithCompletionHandler's completionHandler block has been run. It is unset when the application is backgrounded.
@property (readwrite, getter=isGameCenterAuthenticationComplete) BOOL gameCenterAuthenticationComplete;

// method
+ (GameCenterHelper *)SharedInstance;
- (void)CloseSharedInstance;

- (void)initializeSignIn;
- (void)startQuickMatchGame : (int)chanel;
- (void)dataSend : (NSData*)pData;
- (void)disconnectMatch;

- (void)showLeaderboard;
- (void)showAchievements;

- (void)submitScore : (NSString *)category score:(int)score;
- (void)achievement : (NSString *)pId percent:(float)percent;

- (void)saveGame:(NSData*)data;
- (void)loadGame;

@end

#endif
