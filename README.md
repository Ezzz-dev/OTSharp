OTSharp
=======

Tibia Game server developed in C# to use with Tibia 7.6

Current Features
=======
   * Handle incoming connections on a different thread
   * Handle connection packets
   * Disconnecting of an active connection
   * Logging in (Make sure no characters are on the same tile)
   * Multiple characters can login and walk
   * Known Creatures (150 max)
   * Using Tibia 7.6 (Login server & Game server)

Where do events go?
=======
   * Inside every class, do not handle events in Game, always try to handle events in it's respective owner class, e.g; a creature walking event should be handled on the Tile class.
  
  
