﻿using Couchbase.Core;
using MattsTwitchBot.Core.CommandQuery.Commands;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.CommandTests
{
    [TestFixture]
    public class ShoutOutTests
    {
        private Mock<ITwitchClient> _mockClient;

        [SetUp]
        public void Setup()
        {
            _mockClient = new Mock<ITwitchClient>();
        }

        [Test]
        public void ShoutOut_should_not_do_anything_for_non_subscribers()
        {
            // arrange
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("NonSub")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithIsSubscriber(false)
                .Build();
            var shout = new ShoutOut(chatMessage, _mockClient.Object);

            // act
            shout.Execute();

            // assert
            _mockClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false), Times.Never);
        }

        [Test]
        public void ShoutOut_will_do_nothing_if_no_name_is_given()
        {
            // arrange
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesntmatter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithIsSubscriber(true)
                .WithMessage("!so")
                .Build();
            var shout = new ShoutOut(chatMessage, _mockClient.Object);

            // act
            shout.Execute();

            // assert
            _mockClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false), Times.Never);
        }

        [Test]
        public void ShoutOut_will_shout_the_given_username()
        {
            // arrange
            var userName = "shoutyMcShoutface";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesntmatter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithIsSubscriber(true)
                .WithMessage($"!so {userName}")
                .Build();
            var shout = new ShoutOut(chatMessage, _mockClient.Object);
            var expectedMessage = $"Hey everyone, check out @{userName}'s Twitch stream at https://twitch.tv/{userName}";

            // act
            shout.Execute();

            // assert
            _mockClient.Verify(x => x.SendMessage(It.IsAny<string>(), expectedMessage, false), Times.Once);
        }
    }
}