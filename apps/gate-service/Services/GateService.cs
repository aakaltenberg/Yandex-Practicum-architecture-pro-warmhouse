using GateService.Controllers;
using GateService.Models;
using GateService.Repositories;
namespace GateService.Services
{
    public class GateService : IGateService
    {
        private readonly IGateRepository _repository;
        private readonly IMessagePublisher _publisher;

        public GateService(IGateRepository repository, IMessagePublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task<List<Gate>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<Gate?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<Gate> CreateAsync(GateCreateDto dto)
        {
            var gate = new Gate
            {
                Id = dto.Id,
                Name = dto.Name,
                Location = dto.Location,
                Description = dto.Description,
                HouseId = dto.HouseId,
                Status = "closed",
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            var created = await _repository.CreateAsync(gate);
            // Публикуем событие создания
            await _publisher.PublishAsync("event.device", new DeviceStateChangedEvent
            {
                DeviceId = created.Id.ToString(),
                HouseId = created.HouseId,
                DeviceType = "gate",
                Status = created.Status,
                Value = null,
                Unit = null,
                Timestamp = created.LastUpdated
            });
            return created;
        }

        public async Task<Gate?> UpdateAsync(int id, GateUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            var updatedGate = new Gate
            {
                Name = dto.Name ?? existing.Name,
                Location = dto.Location ?? existing.Location,
                Description = dto.Description ?? existing.Description,
                HouseId = dto.HouseId ?? existing.HouseId,
                Status = existing.Status,
                CreatedAt = existing.CreatedAt
            };
            var updated = await _repository.UpdateAsync(id, updatedGate);
            if (updated != null)
            {
                await _publisher.PublishAsync("event.device", new DeviceStateChangedEvent
                {
                    DeviceId = updated.Id.ToString(),
                    HouseId = updated.HouseId,
                    DeviceType = "gate",
                    Status = updated.Status,
                    Value = null,
                    Unit = null,
                    Timestamp = updated.LastUpdated
                });
            }
            return updated;
        }

        public async Task<bool> OpenGateAsync(int id)
        {
            var success = await _repository.UpdateStatusAsync(id, "open");
            if (success)
            {
                var gate = await _repository.GetByIdAsync(id);
                if (gate != null)
                {
                    await _publisher.PublishAsync("event.device", new DeviceStateChangedEvent
                    {
                        DeviceId = gate.Id.ToString(),
                        HouseId = "",
                        DeviceType = "gate",
                        Status = "open",
                        Value = null,
                        Unit = null,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }
            return success;
        }

        public async Task<bool> CloseGateAsync(int id)
        {
            var success = await _repository.UpdateStatusAsync(id, "closed");
            if (success)
            {
                var gate = await _repository.GetByIdAsync(id);
                if (gate != null)
                {
                    await _publisher.PublishAsync("event.device", new DeviceStateChangedEvent
                    {
                        DeviceId = gate.Id.ToString(),
                        HouseId = "",
                        DeviceType = "gate",
                        Status = "closed",
                        Value = null,
                        Unit = null,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }
            return success;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var gate = await _repository.GetByIdAsync(id);
            if (gate == null) return false;

            var deleted = await _repository.DeleteAsync(id);
            if (deleted)
            {
                await _publisher.PublishAsync("event.device", new DeviceStateChangedEvent
                {
                    DeviceId = gate.Id.ToString(),
                    HouseId = "",
                    DeviceType = "gate",
                    Status = "deleted",
                    Value = null,
                    Unit = null,
                    Timestamp = DateTime.UtcNow
                });
            }
            return deleted;
        }
    }
}
