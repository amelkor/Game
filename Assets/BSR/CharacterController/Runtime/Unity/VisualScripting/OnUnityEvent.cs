using System;
using System.Reflection;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace Unity.VisualScripting
{
    internal class OnUnityEventData : EventUnit<EventData>.Data
    {
        public object EventListener { get; set; }
    }

    public class EventData
    {
        public object Value0 { get; set; }
        public object Value1 { get; set; }
        public object Value2 { get; set; }
        public object Value3 { get; set; }
    }

    [UnitTitle("On Unity Event")]
    [UnitCategory("Events")]
    public class OnUnityEvent : EventUnit<EventData>
    {
        protected override bool register => false;

        [DoNotSerialize]
        public ValueInput UnityEvent;

        public Type Type { get; private set; }

        public override IGraphElementData CreateData()
        {
            return new OnUnityEventData();
        }

        protected override void Definition()
        {
            base.Definition();

            UnityEvent = ValueInput<UnityEventBase>("event");

            if (Type != null)
            {
                var genericArguments = Type.GetGenericArguments();
                for (var i = 0; i < genericArguments.Length; i++)
                {
                    var arg = genericArguments[i];
                    ValueOutput(arg, $"{arg.Name}{i}");
                }
            }
        }

        public override void StartListening(GraphStack stack)
        {
            var data = GetData(stack);

            if (data.EventListener != null || !UnityEvent.hasValidConnection)
                return;

            UpdatePorts();

            var stackRef = stack.ToReference();
            var eventBase = Flow.FetchValue<UnityEventBase>(UnityEvent, stackRef);
            var method = Type.GetMethod(nameof(UnityEngine.Events.UnityEvent.AddListener));
            var delegateType = method?.GetParameters()[0].ParameterType;

            data.EventListener = CreateAction(delegateType, stackRef);

            method?.Invoke(eventBase, new[] { data.EventListener });
        }

        public override void StopListening(GraphStack stack)
        {
            var data = GetData(stack);

            if (data.EventListener == null)
                return;

            var stackRef = stack.ToReference();
            var eventBase = Flow.FetchValue<UnityEventBase>(UnityEvent, stackRef);
            var method = Type.GetMethod(nameof(UnityEngine.Events.UnityEvent.RemoveListener));
            method?.Invoke(eventBase, new[] { data.EventListener });

            data.EventListener = null;
        }

        public void UpdatePorts()
        {
            Type = GetEventType();
            Define();
        }

        private Type GetEventType()
        {
            var eventType = UnityEvent?.connection?.source?.type;

            while (eventType != null && eventType.BaseType != typeof(UnityEventBase))
            {
                eventType = eventType.BaseType;
            }

            return eventType;
        }

        private object CreateAction(Type delegateType, GraphReference reference)
        {
            var numParams = delegateType.GetGenericArguments().Length;

            if (numParams == 0)
            {
                void Action()
                {
                    Trigger(reference, new EventData());
                }

                return (UnityAction)Action;
            }

            var methodName = numParams switch
            {
                1 => nameof(OneParamHandler),
                2 => nameof(TwoParamsHandler),
                3 => nameof(ThreeParamsHandler),
                _ => nameof(FourParamsHandler)
            };

            var method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            return method?.MakeGenericMethod(delegateType.GetGenericArguments()).Invoke(this, new object[] { reference });
        }

        internal UnityAction<T> OneParamHandler<T>(GraphReference reference)
        {
            return arg0 =>
            {
                Trigger(reference, new EventData
                {
                    Value0 = arg0
                });
            };
        }

        internal UnityAction<T0, T1> TwoParamsHandler<T0, T1>(GraphReference reference)
        {
            return (arg0, arg1) =>
            {
                Trigger(reference, new EventData
                {
                    Value0 = arg0,
                    Value1 = arg1
                });
            };
        }

        internal UnityAction<T0, T1, T2> ThreeParamsHandler<T0, T1, T2>(GraphReference reference)
        {
            return (arg0, arg1, arg2) =>
            {
                Trigger(reference, new EventData
                {
                    Value0 = arg0,
                    Value1 = arg1,
                    Value2 = arg2
                });
            };
        }

        internal UnityAction<T0, T1, T2, T3> FourParamsHandler<T0, T1, T2, T3>(GraphReference reference)
        {
            return (arg0, arg1, arg2, arg3) =>
            {
                Trigger(reference, new EventData
                {
                    Value0 = arg0,
                    Value1 = arg1,
                    Value2 = arg2,
                    Value3 = arg3
                });
            };
        }

        protected override void AssignArguments(Flow flow, EventData args)
        {
            var numOutputs = valueOutputs.Count;

            if (numOutputs > 0) flow.SetValue(valueOutputs[0], args.Value0);
            if (numOutputs > 1) flow.SetValue(valueOutputs[1], args.Value1);
            if (numOutputs > 2) flow.SetValue(valueOutputs[2], args.Value2);
            if (numOutputs > 3) flow.SetValue(valueOutputs[3], args.Value3);
        }

        private OnUnityEventData GetData(GraphPointer stack)
        {
            return stack.GetElementData<OnUnityEventData>(this);
        }
    }
}