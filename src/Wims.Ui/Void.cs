﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Wims.Ui
{
	public static class VoidExtensions
	{
		public static IObservable<Void> ToVoid(this IObservable<System.Reactive.Unit> @this) =>
			@this.Select(_ => Void.Value);

		public static IObservable<Void> ToVoid<T>(this IObservable<T> @this) => @this.Select(_ => Void.Value);

		public static Task<Void> ToVoid(this Task<System.Reactive.Unit> @this) => Task.FromResult(Void.Value);
	}

	/// <summary>
	/// A unified <code>Unit</code> type.
	/// Taken from https://github.com/jbogard/MediatR/blob/master/src/MediatR/Unit.cs
	/// </summary>
	public struct Void : IEquatable<Void>, IComparable<Void>, IComparable
	{
		public static implicit operator System.Reactive.Unit(Void unit) => System.Reactive.Unit.Default;
		public static implicit operator Void(System.Reactive.Unit unit) => Void.Default;

		public static implicit operator MediatR.Unit(Void unit) => MediatR.Unit.Value;
		public static implicit operator Void(MediatR.Unit unit) => Void.Default;

		/// <summary>
		/// Default and only value of the <see cref="Unit"/> type.
		/// </summary>
		public static readonly Void Value = new Void();

		/// <summary>
		/// Gets the single <see cref="Unit"/> value.
		/// </summary>
		public static Void Default = default;

		/// <summary>
		/// Task from a <see cref="Unit"/> type.
		/// </summary>
		public static readonly Task<Void> Task = System.Threading.Tasks.Task.FromResult(Value);

		/// <summary>
		/// An observable that ticks a single Void value.
		/// </summary>
		public static readonly IObservable<Void> Observable = System.Reactive.Linq.Observable.Return(Value);

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared.
		/// The return value has the following meanings:
		///  - Less than zero: This object is less than the <paramref name="other" /> parameter.
		///  - Zero: This object is equal to <paramref name="other" />.
		///  - Greater than zero: This object is greater than <paramref name="other" />.
		/// </returns>
		public int CompareTo(Void other)
		{
			return 0;
		}

		/// <summary>
		/// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared.
		/// The return value has these meanings:
		///  - Less than zero: This instance precedes <paramref name="obj" /> in the sort order.
		///  - Zero: This instance occurs in the same position in the sort order as <paramref name="obj" />.
		///  - Greater than zero: This instance follows <paramref name="obj" /> in the sort order.
		/// </returns>
		int IComparable.CompareTo(object obj)
		{
			return 0;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			return 0;
		}

		/// <summary>
		/// Determines whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// <c>true</c> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <c>false</c>.
		/// </returns>
		public bool Equals(Void other)
		{
			return true;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			return obj is Void;
		}

		/// <summary>
		/// Determines whether the <paramref name="first"/> object is equal to the <paramref name="second"/> object.
		/// </summary>
		/// <param name="first">The first object.</param>
		/// <param name="second">The second object.</param>
		/// <c>true</c> if the <paramref name="first"/> object is equal to the <paramref name="second" /> object; otherwise, <c>false</c>.
		public static bool operator ==(Void first, Void second)
		{
			return true;
		}

		/// <summary>
		/// Determines whether the <paramref name="first"/> object is not equal to the <paramref name="second"/> object.
		/// </summary>
		/// <param name="first">The first object.</param>
		/// <param name="second">The second object.</param>
		/// <c>true</c> if the <paramref name="first"/> object is not equal to the <paramref name="second" /> object; otherwise, <c>false</c>.
		public static bool operator !=(Void first, Void second)
		{
			return false;
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			return "()";
		}
	}
}