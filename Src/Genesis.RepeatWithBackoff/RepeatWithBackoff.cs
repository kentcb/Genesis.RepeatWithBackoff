namespace System.Reactive.Linq
{
    using System;
    using Concurrency;

    /// <summary>
    /// Provides the <see cref="RepeatWithBackoff"/> extension method.
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// The default strategy for <see cref="RepeatWithBackoff"/>, which waits n^2 seconds between each repetition, or 180 seconds, whichever is smaller.
        /// </summary>
        public static readonly Func<int, TimeSpan> DefaultStrategy = n => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, n), 180));

        /// <summary>
        /// Repeats an observable sequence, using the provided strategy to determine how long to wait between repetitions.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This extension method can be used to repeat any source pipeline a specified number of times, with a custom
        /// wait period between those repetitions. The <paramref name="repeatCount"/> parameter determines the maximum number of repetitions. The
        /// default value is <see langword="null"/>, which means there is no maximum (will repeat indefinitely). The
        /// <paramref name="strategy"/> parameter dictates the period between repetitions, and it defaults to <see cref="DefaultStrategy"/>.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">
        /// The source type.
        /// </typeparam>
        /// <param name="this">
        /// The source observable.
        /// </param>
        /// <param name="repeatCount">
        /// How many times to repeat, or <see langword="null"/> to repeat indefinitely.
        /// </param>
        /// <param name="strategy">
        /// The strategy to use when repeating, or <see langword="null"/> to use <see cref="DefaultStrategy"/>.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler to use for delays, or <see langword="null"/> to use the default scheduler.
        /// </param>
        /// <returns>
        /// An observable that will repeat a source observable according to the timing dictated by <paramref name="strategy"/>.
        /// </returns>
        public static IObservable<T> RepeatWithBackoff<T>(
            this IObservable<T> @this,
            int? repeatCount = null,
            Func<int, TimeSpan> strategy = null,
            IScheduler scheduler = null)
        {
            strategy = strategy ?? DefaultStrategy;
            scheduler = scheduler ?? DefaultScheduler.Instance;

            var attempt = 0;
            var pipeline = Observable
                .Defer(
                    () =>
                        ((attempt++ == 0) ?
                            @this :
                            @this
                                .DelaySubscription(strategy(attempt - 1), scheduler)));

            if (repeatCount.HasValue)
            {
                return pipeline
                    .Repeat(repeatCount.Value);
            }
            else
            {
                return pipeline
                    .Repeat();
            }
        }
    }
}