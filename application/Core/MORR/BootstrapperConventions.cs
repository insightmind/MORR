using System;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using MORR.Core.Data.Transcoding;
using MORR.Shared.Configuration;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Modules;

namespace MORR.Core
{
    public static class BootstrapperConventions
    {
        private static bool IsQueueInterfaceType(Type typeToCheck, Type queueInterfaceType)
        {
            return typeToCheck.IsInterface && typeToCheck.IsGenericType &&
                   typeToCheck.GetGenericTypeDefinition() == queueInterfaceType;
        }

        private static bool ImplementsQueueType(Type typeToCheck, Type queueType)
        {
            return !typeToCheck.IsAbstract && typeToCheck
                                              .GetInterfaces()
                                              .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == queueType);
        }

        /// <summary>
        ///     Gets a registration builder that contains all composition conventions
        /// </summary>
        /// <returns>The <see cref="RegistrationBuilder" /> containing all composition conventions.</returns>
        public static RegistrationBuilder GetRegistrationBuilder()
        {
            var registrationBuilder = new RegistrationBuilder();

            // Export all modules as IModule
            registrationBuilder.ForTypesDerivedFrom<IModule>().Export<IModule>();

            // Export implementers of IConfiguration as themselves and as IConfiguration
            registrationBuilder.ForTypesDerivedFrom<IConfiguration>().Export<IConfiguration>().Export();

            // Export implementers of IReadOnlyEventQueue<'Event'> as IReadOnlyEventQueue<'Event'> and IReadOnlyEventQueue<Event>
            registrationBuilder.ForTypesMatching(x => ImplementsQueueType(x, typeof(IReadOnlyEventQueue<>)))
                               .Export<IReadOnlyEventQueue<Event>>();
            registrationBuilder.ForTypesMatching(x => IsQueueInterfaceType(x, typeof(IReadOnlyEventQueue<>)))
                               .Export(x => x.Inherited());

            // Export implementers of ISupportDeserializationEventQueue<'Event'> as ISupportDeserializationEventQueue<'Event'>
            // and ISupportDeserializationEventQueue<Event>
            registrationBuilder
                .ForTypesMatching(x => ImplementsQueueType(x, typeof(ISupportDeserializationEventQueue<>)))
                .Export<ISupportDeserializationEventQueue<Event>>();
            registrationBuilder
                .ForTypesMatching(x => IsQueueInterfaceType(x, typeof(ISupportDeserializationEventQueue<>)))
                .Export(x => x.Inherited());

            // Export producers (e.g. IReadOnlyEventQueue or ISupportDeserializationEventQueue) as themselves
            registrationBuilder.ForTypesMatching(x =>
                                                     ImplementsQueueType(x, typeof(IReadOnlyEventQueue<>)) ||
                                                     ImplementsQueueType(
                                                         x, typeof(ISupportDeserializationEventQueue<>)))
                               .Export();

            // Export implementers of IEncodableEventQueue<'Event'> as IEncodableEventQueue<'Event'> and themselves
            registrationBuilder.ForTypesMatching(x => ImplementsQueueType(x, typeof(IEncodableEventQueue<>)))
                               .Export();
            registrationBuilder.ForTypesMatching(x => IsQueueInterfaceType(x, typeof(IEncodableEventQueue<>)))
                               .Export(x => x.Inherited());

            // Export implementers of IDecodeableEventQueue<'Event'> as IDecodeableEventQueue<'Event'> and themselves
            registrationBuilder.ForTypesMatching(x => ImplementsQueueType(x, typeof(IDecodableEventQueue<>)))
                               .Export();
            registrationBuilder.ForTypesMatching(x => IsQueueInterfaceType(x, typeof(IDecodableEventQueue<>)))
                               .Export(x => x.Inherited());

            // Export implementers of IEncoder as IEncoder
            registrationBuilder.ForTypesDerivedFrom<IEncoder>().Export<IEncoder>(x => x.Inherited());

            // Export implementers of IDecoder as IDecoder
            registrationBuilder.ForTypesDerivedFrom<IDecoder>().Export<IDecoder>(x => x.Inherited());

            return registrationBuilder;
        }
    }
}