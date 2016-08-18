namespace Genesis.RepeatWithBackoff.UnitTests
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using Microsoft.Reactive.Testing;
    using Xunit;

    public sealed class RepeatWithBackoffFixture
    {
        [Fact]
        public void repeats_indefinitely_if_no_repeat_count_specified()
        {
            var repetitions = 0;
            var scheduler = new TestScheduler();
            var source = Observable
                .Defer(
                    () =>
                    {
                        ++repetitions;
                        return Observable.Return(Unit.Default);
                    });
            var sut = source
                .RepeatWithBackoff(scheduler: scheduler)
                .Subscribe(
                    _ => { },
                    ex => { });
            scheduler.AdvanceBy(TimeSpan.FromDays(1));

            Assert.Equal(486, repetitions);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(42)]
        public void repeat_count_determines_how_many_times_to_repeat(int repeatCount)
        {
            var repetitions = 0;
            var scheduler = new TestScheduler();
            var source = Observable
                .Defer(
                    () =>
                    {
                        ++repetitions;
                        return Observable.Return(Unit.Default);
                    });
            var sut = source
                .RepeatWithBackoff(repeatCount, scheduler: scheduler)
                .Subscribe(
                    _ => { },
                    ex => { });
            scheduler.AdvanceUntilEmpty();

            Assert.Equal(repeatCount, repetitions);
        }

        [Fact]
        public void default_strategy_is_exponential_backoff_to_a_maximum_of_three_minutes()
        {
            var repetitions = 0;
            var scheduler = new TestScheduler();
            var source = Observable
                .Defer(
                    () =>
                    {
                        ++repetitions;
                        return Observable.Return(Unit.Default);
                    });
            var sut = source
                .RepeatWithBackoff(100, scheduler: scheduler)
                .Subscribe(
                    _ => { },
                    ex => { });
            Assert.Equal(1, repetitions);

            var @try = 1;

            for (var i = 0; i < 7; ++i)
            {
                scheduler.AdvanceBy(TimeSpan.FromSeconds(Math.Pow(2, @try)) - TimeSpan.FromMilliseconds(1));
                Assert.Equal(@try, repetitions);
                scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));
                Assert.Equal(++@try, repetitions);
            }

            // we've reached the 3 minute maximum delay
            for (var i = 0; i < 5; ++i)
            {
                scheduler.AdvanceBy(TimeSpan.FromMinutes(3) - TimeSpan.FromMilliseconds(1));
                Assert.Equal(@try, repetitions);
                scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));
                Assert.Equal(++@try, repetitions);
            }
        }

        [Fact]
        public void strategy_determines_time_between_repetitions()
        {
            var repetitions = 0;
            var scheduler = new TestScheduler();
            var source = Observable
                .Defer(
                    () =>
                    {
                        ++repetitions;
                        return Observable.Return(Unit.Default);
                    });
            var sut = source
                .RepeatWithBackoff(100, strategy: n => TimeSpan.FromSeconds(n), scheduler: scheduler)
                .Subscribe(
                    _ => { },
                    ex => { });
            Assert.Equal(1, repetitions);

            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(999));
            Assert.Equal(1, repetitions);
            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));
            Assert.Equal(2, repetitions);

            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1999));
            Assert.Equal(2, repetitions);
            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));
            Assert.Equal(3, repetitions);

            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(2999));
            Assert.Equal(3, repetitions);
            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));
            Assert.Equal(4, repetitions);
        }
    }
}