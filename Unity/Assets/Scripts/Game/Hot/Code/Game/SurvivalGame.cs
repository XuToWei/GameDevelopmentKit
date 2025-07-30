﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEngine;

namespace Game.Hot
{
    public class SurvivalGame : GameBase
    {
        private float m_ElapseSeconds = 0f;

        public override GameMode GameMode
        {
            get
            {
                return GameMode.Survival;
            }
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            base.Update(elapseSeconds, realElapseSeconds);

            m_ElapseSeconds += elapseSeconds;
            if (m_ElapseSeconds >= 1f)
            {
                m_ElapseSeconds = 0f;
                float randomPositionX = m_SceneBackground.EnemySpawnBoundary.bounds.min.x + m_SceneBackground.EnemySpawnBoundary.bounds.size.x * (float)Utility.Random.GetRandomDouble();
                float randomPositionZ = m_SceneBackground.EnemySpawnBoundary.bounds.min.z + m_SceneBackground.EnemySpawnBoundary.bounds.size.z * (float)Utility.Random.GetRandomDouble();
                GameEntry.Entity.ShowAsteroid(new AsteroidData(GameEntry.Entity.GenerateSerialId(), 60000 + Utility.Random.GetRandom(HotEntry.Tables.DTAsteroid.DataList.Count))
                {
                    Position = new Vector3(randomPositionX, 0f, randomPositionZ),
                });
            }
        }
    }
}
