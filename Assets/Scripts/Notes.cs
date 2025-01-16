/*
 * 
 *            VG
 *                      written report 2 to 4 pages detailing game design and network solutions challenges etc
 *          
 * 
 * do notes here
 * 
 * area control
 *      teams try to get the most areas controlled in a time limit
 *      is control taken instantly, or does it take time?
 * 
 *
 *      should players be able to push eachother?
 *      ---------------------------------------------------------------
 *      Tag?
 *          1 person is IT for each team?
 *                  IT cannot take territory, other teammates can
 *                  tagging ppl builds their team?
 *                  other players are
 *    ----------------------------------------------------------------------------------              
 *          1 person is IT at a time
 *                  tagging a person takes their territory, and makes them it?
 *              
 *          Tagging a player makes you swap? everyone is it?
 *              
 *      sardines variation
 *          one player hides. other players must be touching the hider or another player that is touching the hider to get a point at the end of the round
 *              last person to find them is next it, or random selection from players who failed to find them
 *              
 *          players should make sounds when they bump into things, maybe they join a "team" of hiders while in contact with another hider
 */


/*
 * 
 * max chaos territory tag
 * 
 *          MIN 2 teams
 *              3 players
 *              consider AI IT bots
 *          
 *          
 *          player 
 *              team
 *              network stuff
 *              character controller stuff
 *              bool IsIT
 *              
 *          team 
 *              players list
 *              team color
 *              points
 *              
 *          area of controll areas
 *              current team alignment
 *              thing to test against to take controll (time spent? points assigned from a team?)
 *              
 *              detect team and number of players(not IT) inside
 *              timer for controll claim, should be short
 *              (maybe closer to the center gives more controll points per tick?)
 *              
 *          gameLogic
 *              session timer
 *              list of teams
 *              list of controll areas
 *              
 *              at end of session loop through controll areas and assign win to teams based on # of territories
 *              
 *              
 *              
 *              
 */


/*
 * LECTURE NOTES ON CLIENT-SERVER AUTHORITY ETC
 * 
 * 
 * NetworkManager.Singleton.OnClientConnectedCallback +=    this fires when a client connects!!  needs to have a ulong clientID
 * 
 * when you make a new networkvariable, you can set read/write persmissions
 * 
 * using strings on network need to use fixed string types. dynamic string types are not valid
 * 
 * player spawning in calls a function on network manager that saves the local player object  to NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
 * 
 */