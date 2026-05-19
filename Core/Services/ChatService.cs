using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Rafiq.Api.Services.Abstractions;
using Shared;

namespace Services
{
    public class ChatService : IChatService
    {
        public ChatService(IUnitOfWork unitOfWork, ILogger<ChatService> logger)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
        }

        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<ChatService> _logger;



        // 1. بدء شات جديد وتنبيه سيرفر الـ AI يصفّر الذاكرة
        public async Task<string> StartNewChatAsync(string identityUserId)
        {
            var patient = (await unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return null;

            var sessionId = Guid.NewGuid().ToString();

            // نضرب الـ URL بتاع الـ AI (بورت 8000 مثلاً) عشان يمسح الميموري للـ Session دي
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://rifling-zap-tidings.ngrok-free.dev"); // حط الـ API URL بتاع الـ AI هنا
                var response = await client.PostAsJsonAsync("/chat/new", new { session_id = sessionId });

                if (response.IsSuccessStatusCode)
                {
                    var session = new ChatSession { SessionId = sessionId, PatientId = patient.Id };
                    await unitOfWork.GetRepository<ChatSession, int>().AddAsync(session);
                    await unitOfWork.CompleteAsync();
                    return sessionId;
                }
            }
            return null;
        }

        // 2. جلب الشات القديم للمريض
        public async Task<List<ChatMessageDto>> GetChatHistoryAsync(string identityUserId)
        {
            var patient = (await unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return new List<ChatMessageDto>();

            var session = (await unitOfWork.GetRepository<ChatSession, int>().GetAllAsync())
                           .FirstOrDefault(s => s.PatientId == patient.Id && s.IsActive);

            if (session == null) return new List<ChatMessageDto>();

            var messages = (await unitOfWork.GetRepository<ChatMessage, int>().GetAllAsync())
                            .Where(m => m.ChatSessionId == session.Id)
                            .OrderBy(m => m.CreatedAt)
                            .Select(m => new ChatMessageDto
                            {
                                Sender = m.Sender,
                                MessageText = m.MessageText,
                                CreatedAt = m.CreatedAt
                            }).ToList();

            return messages;
        }

        // 3. إنهاء المحادثة وحذف كل البيانات (الـ Popup اللي بعتهولي)
        public async Task<bool> EndChatAsync(string identityUserId)
        {
            var patient = (await unitOfWork.GetRepository<Patient, int>().GetAllAsync())
                           .FirstOrDefault(p => p.userId == identityUserId);

            if (patient == null) return false;

            var session = (await unitOfWork.GetRepository<ChatSession, int>().GetAllAsync())
                           .FirstOrDefault(s => s.PatientId == patient.Id && s.IsActive);

            if (session == null) return false;

            var messagesRepo = unitOfWork.GetRepository<ChatMessage, int>();
            var messages = (await messagesRepo.GetAllAsync()).Where(m => m.ChatSessionId == session.Id).ToList();

            foreach (var msg in messages)
            {
                messagesRepo.Delete(msg.Id);
            }

            unitOfWork.GetRepository<ChatSession, int>().Delete(session.Id);
            return await unitOfWork.CompleteAsync() > 0;
        }

    }
}
