# BottomText

A discord tool for forcing a message to always be the most recent message in a channel.

## Why would you want that?

Mainly for when I'm running D&D, I can keep status information on the bottom.

## How to I install?

[Click Here to go to the download page](https://pikcube.com/BottomText/)

## Why don't you also host a copy on github itself?
I don't want to. I can click one button in VS and it pushes the new version directly to the website. Setting up an automation to also generate a new github release would be really annoying.

## Are there dependencies?

Dotnet version 8, which the installer should download if you don't have 

You'll also need a discord bot token, [which you can make here for free.](https://discord.com/developers/applications). Make sure the bot can send messages and receive messages otherwise you'll have a bad time.

## It says I need a token?

That would be the token for a discord bot in your discord server. See the instructions above.

## I want to change which channel I'm posting in

Close and reopen the app to start a new session.

## What's up with the progress bars at the bottom?

In an effort to not spam your channel, there's a delay on posting a new message and updating an old message that resets to 0 every time a change is made. They tick up when there's been no activity.

## How do I send an emoji?
Just type it in the textbox.

## How do I type a discord emote?
Just type in the character code for it. It'll likely be ```:xxxxx:```.

## How do I make this @ mention someone?

Are you sure you want that? That could be a lot of pings. In anycase the syntax for an @ mention is based on the user's uid. Fastest way to find it is to @ mention them, but put a back slash in front of it. The sent message will look funky, that's what you want.

## What if I run two of these, who will win?

Good question. They don't respond to bot messages at all so it's just a case of who sends their message first...but why do you want that?

## Is there a mac version?

No, although I might port this to Avalonia UI later if people are interestreted.

## Where did your app install to?

It's a click one installer, so it installed in your app directory folder.

## How do I build from source?

Download visual studio, clone the repo, and open the .sln. There aren't any dependencies that aren't just nuget packages.

## Can I fork this?

I guess?

## Can you make a bot token for me?

No.

## Can I send from a user account instead of a bot?

Please don't. I doubt it is impossible but it would be a giant Discord TOS violation. If a message is automated, let people know it is from a bot. That's just good transparancy.
