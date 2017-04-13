![Logo](Art/Logo150x150.png "Logo")

# Genesis.RepeatWithBackoff

[![Build status](https://ci.appveyor.com/api/projects/status/55k9at7jcdlw6chq?svg=true)](https://ci.appveyor.com/project/kentcb/genesis-repeatwithbackoff)

## What?

> All Genesis.* projects are formalizations of small pieces of functionality I find myself copying from project to project. Some are small to the point of triviality, but are time-savers nonetheless. They have a particular focus on performance with respect to mobile development, but are certainly applicable outside this domain.
 
**Genesis.RepeatWithBackoff** adds a `RepeatWithBackoff` extension method to observables (based on [this work](https://gist.github.com/atifaziz/c6776b936a36a98a8153) by @niik). As the name suggests, the `RepeatWithBackoff` method makes it simple to repeat an observable with a variable delay between repetitions.

**Genesis.RepeatWithBackoff** is delivered as a netstandard 1.0 binary.

## Why?

Rx makes it simple to repeat a pipeline, either a set number of times or indefinitely. However, it provides no in-built means of repeating with a delay between each repetition.

## Where?

The easiest way to get **Genesis.RepeatWithBackoff** is via [NuGet](http://www.nuget.org/packages/Genesis.RepeatWithBackoff/):

```PowerShell
Install-Package Genesis.RepeatWithBackoff
```

## How?

**Genesis.RepeatWithBackoff** adds a single `RepeatWithBackoff` extension method to your observable sequences. It's defined in the `System.Reactive.Linq` namespace, so you'll generally have access to it if you're already using LINQ to Rx.

Here are some examples:

```C#
// repeat indefinitely, backing off exponentially to a maximum of 3 minutes
someObservable
    .RepeatWithBackoff();

// repeat 3 times, backing off exponentially
someObservable
    .RepeatWithBackoff(repeatCount: 3);

// repeat 3 times, with 3 seconds between each repetition
someObservable
    .RepeatWithBackoff(
        repeatCount: 3,
        strategy: n => TimeSpan.FromSeconds(3));

// repeat with a custom scheduler (useful for tests)
someObservable
    RepeatWithBackoff(scheduler: s);
```

## Who?

**Genesis.RepeatWithBackoff** is created and maintained by [Kent Boogaart](http://kent-boogaart.com). However, it is based on [original code](https://gist.github.com/atifaziz/c6776b936a36a98a8153) is by @niik. Issues and pull requests are welcome.