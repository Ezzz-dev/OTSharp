OTSharp
=======

Tibia Game server developed in C# to use with Tibia 7.6

Features
=======
   * Handle incoming connections on a separate thread
   * Talking
   * Walking
   * Channels
   * Rule Violation Channel
   * Creature Events
   * Tasking

Where do events go?
=======
   * Inside every class, do not handle events in Game, always try to handle events in it's respective owner class, e.g; a creature walking event should be handled on the Tile class.
  
  
