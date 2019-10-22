﻿using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core
{
    // builds mediatr requests given a twitch chat message
    public class TwitchCommandRequestFactory
    {
        private readonly IMediator _mediator;

        public TwitchCommandRequestFactory(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IRequest> BuildCommand(ChatMessage chatMessage)
        {
            var messageText = chatMessage.Message;
            switch (messageText)
            {
                case var x when x.StartsWith("!help"):
                    return new Help(chatMessage);
                case var x when x.StartsWith("!currentproject"):
                    return new SayCurrentProject(chatMessage);
                case var x when x.StartsWith("!setcurrentproject"):
                    return new SetCurrentProject(chatMessage);
                case var x when x.StartsWith("!so "):
                    return new ShoutOut(chatMessage);
                case var x when x.StartsWith("!profile"):
                    return new ModifyProfile(chatMessage);
                case var x when await IsASoundEffect(x):
                    return new SoundEffect(x.Replace("!", ""));
                default:
                    return new StoreMessage(chatMessage);
            }
        }

        private async Task<bool> IsASoundEffect(string commandText)
        {
            var validSoundEffects = await _mediator.Send<ValidSoundEffects>(new SoundEffectLookup());
            return validSoundEffects.IsValid(commandText);
        }
    }
}