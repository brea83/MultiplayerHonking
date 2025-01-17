Project for my Network Programming class.

uses unity starter assets as well as a honking sound effect from freesound.org

    Bird Honk - 1.wav by SpaceJoe -- https://freesound.org/s/510940/ -- License: Creative Commons 0

Players can honk, walk, sprint, jump

      character controller is unity's starter kit, not my work. though I did tweak how it plays footstep sound effects
      I added a honk button to the player input which plays a sound effect and causes text to display over the player's head for a moment

Players claim glowing areas by walking into them. 

---------------------------------------------------------------
future plans include 

  lights and more sound effects
  
  time spent in a region contributes to claiming it so that multiple players 
  
      can pile into an area and push eachother out to claim it
      
  points read out: 
  
      currently I haven't read enough about unity netcode for gameobjects's rpc 
      system to know how to correctly counterballance how it is sending multiple requests, 
      without that I can't increment a team's points on area claimed. 
      I ran out of time for finding other work arounds.
      
