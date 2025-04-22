# Game Feature Services

This directory is intended for services that are specific to the Game feature.

## Feature-Based Service Organization

In our architecture:
- Global application services (cross-cutting concerns like logging, configuration, and platform services) are placed in the root `/Services` directory
- Services that are specific to a feature are placed in that feature's Services directory (like this one)

## When to Add Services Here

Add a service class to this directory when:
1. The service is only used by the Game feature
2. The service implements Game-specific business logic
3. The service wouldn't make sense as a global application concern

## Examples of Potential Game Services

- `GameStateService` - Handle saving/loading game state
- `GameStatisticsService` - Track and calculate game statistics
- `GameAchievementService` - Manage game achievements
- `GameSoundService` - Handle game-specific sound effects
