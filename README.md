# Asteroids

Classic Asteroids game with a 3D flight-sim perspective, written in Monogame. You are the pilot of a ship trying to save your planet from giant lava rocks that have erupted from a nearby space volcano. Destroy them!

## Controls
Mouse - steer ship

W - Thrust

A/D - Roll

Left Mouse Button - Fire Torpedo (cooldown of 2 seconds)

Q - Activate Shrink

## The World
Asteroids come in 3 sizes, small, medium, and large, and spawn in random shapes and sizes. Hit one, you die and lose a life. Lose all three of your lives and it's GG.

#### Powerups
Powerups occasionally spawn in random positions of the world.

**Shields** allow you to hit an asteroid without dying. The shield is consumed once used.

**Shrinks** decrease the size of your ship, allowing you to get out of sticky situations. The shrink lasts 10 seconds before you revert to normal size.

## Environment
- Windows 10
- Visual Studio 2015
- Monogame Development Branch (3.5+ essentially), otherwise it doesn't work with VS 2015
- Other versions of Windows or VS may work, I have not tested them.

## Setup
- Install Monogame from the development branch
- Clone repo, open in Visual Studio, verify everything builds with the Monogame Pipeline Tool
- Run it in Visual Studio

## Known Unresolved Issues
- Occasionally when two asteroids collide the game hangs and nothing is drawn. I have not been able to figure out why this happens. Press "R" to fix this. Not pretty, but it works.
- Occasionally when exiting the game a NullReferenceException is thrown.