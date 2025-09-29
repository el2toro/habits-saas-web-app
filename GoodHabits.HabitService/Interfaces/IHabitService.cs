using GoodHabits.Database.Entities;

namespace GoodHabits.HabitService.Interfaces;

public interface IHabitService
{
    Task<Habit> Create(string name, string description);
    Task<Habit> GetById(int id);
    Task<IReadOnlyList<Habit>> GetAll();
}