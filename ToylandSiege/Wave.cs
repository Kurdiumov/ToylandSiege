using System;
using System.Collections.Generic;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class Wave
    {
        public List<Unit> AvailableUnits = new List<Unit>();
        public List<Unit> UnitsInWave = new List<Unit>();

        public double WaveTime;
        public double TimeLeft;
        public TimeSpan WaveStartedTime;

        public Wave()
        {
            
        }

        public void AddUnit(Unit unit)
        {
            AvailableUnits.Add(unit);
        }

        public void RefreshLists()
        {
            AvailableUnits.RemoveAll(item => item == null);
            UnitsInWave.RemoveAll(item => item == null || item.Health <= 0);
        }

        public void AddUnitToField(Unit unit)
        {
            UnitsInWave.Add(unit);
            AvailableUnits.Remove(unit);
        }
    }
}
