# Aatrox

[![Build Status](https://dev.azure.com/allanmercou/Aatrox/_apis/build/status/Kiritsu.Aatrox?branchName=master)](https://dev.azure.com/allanmercou/Aatrox/_build/latest?definitionId=3&branchName=master)

A simple and open source Discord bot made in C# with .NET Core 3.0.

## Features 

The bot has no features for now as I worked on the Core and the Database part before coding any content. However, I plan on implementing these features:

### Todo

* League of Legends user stats, game, and live tracker
* Osu! user stats, score tracker
* Moderation utility commands (message cleanup, kick, ban, mute, etc.)
* RSS Feed subscription
* Twitch live tracking
* Common commands
* Custom commands (with a few variables)
* Discord events logging (join, leave, removed/updated messages, pins, ...) with usage of AuditLogs.

## Requirements

In order to use the bot by yourself, you will need the following components: 
* Git
* .NET Core (>= 3.0) SDK & Runtime
* Visual Studio 2019
* Discord Bot Application
* PostgreSQL Server (>= 10.7)
* Docker & Docker-Compose (facultative)

### Self compilation

1. Clone the current project with `git clone https://github.com/Kiritsu/Aatrox`
2. Update the Disqord submodule with `git submodule update --recursive --remote`
3. Either open the solution and build it or use `dotnet build` to build the solution.

You can publish the app yourself with the command: `--framework netcoreapp3.0 --configuration Release --runtime ubuntu.16.04-x64`

*You will have to adapt the runtime with the one of your choice.*

### Pre-compiled artifact

There will soon be a script for downloading a ready-for-use artifact but you can already download them from [azure-pipelines](https://dev.azure.com/allanmercou/Aatrox/_build?definitionId=3)
