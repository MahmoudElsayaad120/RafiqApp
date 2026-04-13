using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Rafiq.Api.Services
{
    public class ChatService : IChatService
    {
        public ChatService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<ChatService> _logger;


        public ChatService( ILogger<ChatService> logger)
        {
          
            _logger = logger;
        }

        public async Task<ChatResponseDto> ProcessMessageAsync(int userId, string message)
        {
            // Save patient message
            await SaveChatMessageAsync(userId, message, "Patient");

            // Analyze symptoms and determine specialization
            var analysis = AnalyzeSymptoms(message);

            // Generate reassuring reply
            var reply = GenerateReassuringReply(analysis.UrgencyLevel, analysis.RecommendedSpecialization);

            // Save bot reply
            await SaveChatMessageAsync(userId, reply, "Bot");

            return new ChatResponseDto
            {
                Reply = reply,
                RecommendedSpecialization = analysis.RecommendedSpecialization,
                UrgencyLevel = analysis.UrgencyLevel
            };
        }

        public async Task SaveChatMessageAsync(int userId, string message, string sender)
        {
            var chatMessage = new ChatMessage
            {
                UserId = userId,
                Message = message,
                Sender = sender,
                CreatedAt = DateTime.UtcNow
            };

           var chat = unitOfWork.GetRepository<ChatMessage,int>().AddAsyns(chatMessage);
            await unitOfWork.SaveChangesAsync();
        }

        private (string RecommendedSpecialization, string UrgencyLevel) AnalyzeSymptoms(string message)
        {
            var lowerMessage = message.ToLowerInvariant();

            // Determine urgency level
            var urgencyKeywords = new Dictionary<string, string[]>
        {
            { "high", new[] { "chest pain", "difficulty breathing", "severe pain", "unconscious", "bleeding", "heart attack", "stroke" } },
            { "medium", new[] { "fever", "persistent", "worsening", "pain", "infection" } },
            { "low", new[] { "mild", "slight", "checkup", "routine" } }
        };

            var urgencyLevel = "low";
            foreach (var kvp in urgencyKeywords)
            {
                if (kvp.Value.Any(keyword => lowerMessage.Contains(keyword)))
                {
                    urgencyLevel = kvp.Key;
                    break;
                }
            }

            // Determine specialization based on symptoms
            var specializationKeywords = new Dictionary<string, string[]>
        {
            { "Cardiology", new[] { "chest", "heart", "cardiac", "blood pressure", "palpitation", "arrhythmia" } },
            { "Neurology", new[] { "headache", "migraine", "seizure", "dizziness", "numbness", "memory", "brain" } },
            { "Orthopedics", new[] { "bone", "joint", "fracture", "sprain", "back pain", "knee", "shoulder" } },
            { "Dermatology", new[] { "skin", "rash", "acne", "eczema", "dermatitis", "mole", "hair loss" } },
            { "Gastroenterology", new[] { "stomach", "abdominal", "digestion", "nausea", "vomiting", "diarrhea", "constipation" } },
            { "Pulmonology", new[] { "breathing", "cough", "asthma", "lung", "respiratory", "shortness of breath" } },
            { "Ophthalmology", new[] { "eye", "vision", "sight", "glaucoma", "cataract", "retina" } },
            { "ENT", new[] { "ear", "nose", "throat", "hearing", "sinus", "tonsil" } },
            { "Psychiatry", new[] { "anxiety", "depression", "mental", "stress", "mood", "psychiatric" } },
            { "Urology", new[] { "urinary", "kidney", "bladder", "prostate", "urination" } },
            { "Endocrinology", new[] { "diabetes", "thyroid", "hormone", "metabolism", "insulin" } },
            { "Gynecology", new[] { "menstrual", "pregnancy", "gynecological", "pelvic", "ovarian" } }
        };

            var recommendedSpecialization = "General Medicine"; // Default

            foreach (var kvp in specializationKeywords)
            {
                if (kvp.Value.Any(keyword => lowerMessage.Contains(keyword)))
                {
                    recommendedSpecialization = kvp.Key;
                    break;
                }
            }

            return (recommendedSpecialization, urgencyLevel);
        }

        private string GenerateReassuringReply(string urgencyLevel, string specialization)
        {
            var urgencyMessages = new Dictionary<string, string>
        {
            { "high", "I understand your concern. Based on your symptoms, I recommend seeking immediate medical attention. Please visit the nearest emergency room or call emergency services if your condition is severe." },
            { "medium", "Thank you for sharing your symptoms. It's important to consult with a healthcare professional soon. I recommend scheduling an appointment with a specialist." },
            { "low", "Thank you for reaching out. While your symptoms seem mild, it's always good to consult with a healthcare professional for proper evaluation." }
        };

            var baseMessage = urgencyMessages.GetValueOrDefault(urgencyLevel, urgencyMessages["low"]);

            return $"{baseMessage} Based on your description, I recommend consulting with a {specialization} specialist. Would you like me to help you find available doctors in this specialization?";
        }
    }
}
