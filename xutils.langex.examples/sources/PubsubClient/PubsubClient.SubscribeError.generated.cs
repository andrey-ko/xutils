﻿//------------------------------------------------------------------------------
// <auto-generated>                                                       
//     This code was generated by a tool.                                       
//                                                                              
//     Changes to this file may cause incorrect behavior and will be lost if    
//     the code is regenerated.                                                 
// </auto-generated>                                                      
//------------------------------------------------------------------------------
#pragma warning disable 1591
namespace xutils.langex.examples {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	public partial class PubsubClient {
		abstract public partial class SubscribeError {
			public enum Id {
				unsolicited,
				errorOutcome
			}
			public interface IMatch {
				void OnUnsolicited(Unsolicited unsolicited);
				void OnErrorOutcome(ErrorOutcome errorOutcome);
			}
			public interface IMatch<TMatch> {
				TMatch OnUnsolicited(Unsolicited unsolicited);
				TMatch OnErrorOutcome(ErrorOutcome errorOutcome);
			}
			public delegate void OnUnsolicitedCallback(Unsolicited unsolicited);
			public delegate void OnErrorOutcomeCallback(ErrorOutcome errorOutcome);
			public delegate TMatch OnUnsolicitedCallback<TMatch>(Unsolicited unsolicited);
			public delegate TMatch OnErrorOutcomeCallback<TMatch>(ErrorOutcome errorOutcome);
			public readonly Id id;
			public class Unsolicited: SubscribeError {
				public readonly string channel;
				public Unsolicited(string channel): base(Id.unsolicited) {
					this.channel = channel;
				}
				public override void Match(IMatch handler) {
					handler.OnUnsolicited(this);
				}
				public override TMatch Match<TMatch>(IMatch<TMatch> handler) {
					return handler.OnUnsolicited(this);
				}
				public override void Match(OnUnsolicitedCallback unsolicited, OnErrorOutcomeCallback errorOutcome) {
					unsolicited(this);
				}
				public override TMatch Match<TMatch>(OnUnsolicitedCallback<TMatch> unsolicited, OnErrorOutcomeCallback<TMatch> errorOutcome) {
					return unsolicited(this);
				}
				public override bool IsUnsolicited() {
					return true;
				}
				public override Unsolicited AsUnsolicited() {
					return this;
				}
			}
			public class ErrorOutcome: SubscribeError {
				public readonly object request;
				public ErrorOutcome(object request): base(Id.errorOutcome) {
					this.request = request;
				}
				public override void Match(IMatch handler) {
					handler.OnErrorOutcome(this);
				}
				public override TMatch Match<TMatch>(IMatch<TMatch> handler) {
					return handler.OnErrorOutcome(this);
				}
				public override void Match(OnUnsolicitedCallback unsolicited, OnErrorOutcomeCallback errorOutcome) {
					errorOutcome(this);
				}
				public override TMatch Match<TMatch>(OnUnsolicitedCallback<TMatch> unsolicited, OnErrorOutcomeCallback<TMatch> errorOutcome) {
					return errorOutcome(this);
				}
				public override bool IsErrorOutcome() {
					return true;
				}
				public override ErrorOutcome AsErrorOutcome() {
					return this;
				}
			}
			protected SubscribeError(Id id) {
			}
			public abstract void Match(IMatch handler);
			public abstract TMatch Match<TMatch>(IMatch<TMatch> handler);
			public abstract void Match(OnUnsolicitedCallback unsolicited, OnErrorOutcomeCallback errorOutcome);
			public abstract TMatch Match<TMatch>(OnUnsolicitedCallback<TMatch> unsolicited, OnErrorOutcomeCallback<TMatch> errorOutcome);
			public virtual bool IsUnsolicited() {
				return false;
			}
			public virtual bool IsErrorOutcome() {
				return false;
			}
			public virtual Unsolicited AsUnsolicited() {
				return null;
			}
			public virtual ErrorOutcome AsErrorOutcome() {
				return null;
			}
		}
	}
}
