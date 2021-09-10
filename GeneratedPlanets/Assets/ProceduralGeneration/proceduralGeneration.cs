using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class proceduralGeneration : MonoBehaviour
{

    public int radius, dirtThickness;
    public float goldOdds, goldVeinBonus;
    public GameObject Grass, Dirt, Stone, Gold;
    public PolygonCollider2D collider;
    enum objectTypes {
        empty,
        grass,
        dirt,
        stone,
        gold
    };

    void Start() {
        objectTypes[,] planetArray = generatePlanetArray();
        planetArray = addGoldVeins(planetArray);
        createPlanetGameObjects(planetArray);
        generateCollider(planetArray);
    }

    bool anyEdgesOutside(int x, int y, objectTypes[,] planetArray) {
        if (x + 1 == planetArray.GetLength(0)) return true;
        if (x - 1 < 0) return true;
        if (y + 1 == planetArray.GetLength(1)) return true;
        if (y - 1 < 0) return true;
        return false;
    }

    bool isExposedTile(int x, int y, objectTypes[,] planetArray) {
        if (planetArray[x, y] != objectTypes.empty) {
            // This tile is filled
            if (anyEdgesOutside(x, y, planetArray)) {
                return true;
            }
            if (planetArray[x, y + 1] == objectTypes.empty
            || planetArray[x + 1, y] == objectTypes.empty
            || planetArray[x, y - 1] == objectTypes.empty
            || planetArray[x - 1, y] == objectTypes.empty) {
                // At least one of the edges has an air block next to it
                return true;
            }
        }
        return false;
    }

    void generateCollider(objectTypes[,] planetArray) {
        List<Vector2> points = new List<Vector2>();
        for (int x = 0; x <= radius*2; x++) {
            int gameX = convertToGameRepresentation(x);
            for (int y = 0; y <= radius*2; y++) {
                int gameY = convertToGameRepresentation(y);

                if (isExposedTile(x, y, planetArray)) { // If the tile is exposed
                    points.Add(new Vector2(gameX, gameY)); // Add the tile to the list
                }
            }
        }
        Vector2[] pointsArray = points.ToArray();
        collider.SetPath(0, pointsArray);
    }

    int convertToGameRepresentation(int value) {
        return value - radius;
    }

    int convertToArrayRepresentation(int value) {
        return value + radius;
    }

    objectTypes[,] generatePlanetArray() {
        objectTypes[,] terrain = new objectTypes[2*radius + 1, 2*radius + 1];
        for (int x = -radius; x <= radius; x++) {
            int arrayX = convertToArrayRepresentation(x);
            for (int y = -radius; y <= radius; y++) {
                int distanceFromCenter = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)));
                int arrayY = convertToArrayRepresentation(y);
                if (distanceFromCenter == radius) {
                    terrain[arrayX, arrayY] = objectTypes.grass;
                }
                else if (distanceFromCenter < radius - dirtThickness) {
                    terrain[arrayX, arrayY] = objectTypes.stone;
                }
                else if (distanceFromCenter < radius) {
                    terrain[arrayX, arrayY] = objectTypes.dirt;
                }
            }
        }
        return terrain;
    }

    objectTypes[,] addGoldVeins(objectTypes[,] planetArray) {
        for (int x = 0; x <= radius*2; x++) {
            for (int y = 0; y <= radius*2; y++) {
                if (planetArray[x, y] == objectTypes.stone) {
                    float odds = goldOdds;
                    if (y + 1 < planetArray.GetLength(1)) {
                        if (planetArray[x, y + 1] == objectTypes.gold) { // if there is gold above, increase the odds of gold here
                            odds += goldVeinBonus;
                        }
                    }
                    if (x + 1 < planetArray.GetLength(0)) {
                        if (planetArray[x + 1, y] == objectTypes.gold) { // if there is gold to the right, increase the odds of gold here
                            odds += goldVeinBonus;
                        }
                    }
                    if (y > 0) {
                        if (planetArray[x, y - 1] == objectTypes.gold) { // if there is gold below, increase the odds of gold here
                            odds += goldVeinBonus;
                        }
                    }
                    if (x > 0) {
                        if (planetArray[x - 1, y] == objectTypes.gold) { // if there is gold on the left, increase the odds of gold here
                            odds += goldVeinBonus;
                        }
                    }
                    if (Random.value <= odds) { // random chance to spawn gold. Increased odds for veins based on the above
                        planetArray[x, y] = objectTypes.gold;
                    }
                }
            }
        }
        return planetArray;
    }

    void createPlanetGameObjects(objectTypes[,] planetArray) {
        for (int x = 0; x <= radius*2; x++) {
            int gameX = convertToGameRepresentation(x);
            for (int y = 0; y <= radius*2; y++) {
                int gameY = convertToGameRepresentation(y);
                if (planetArray[x, y] == objectTypes.grass) spawnObj(Grass, gameX, gameY);
                if (planetArray[x, y] == objectTypes.dirt) spawnObj(Dirt, gameX, gameY);
                if (planetArray[x, y] == objectTypes.stone) spawnObj(Stone, gameX, gameY);
                if (planetArray[x, y] == objectTypes.gold) spawnObj(Gold, gameX, gameY);
            }
        }
    }

    void spawnObj(GameObject obj, int width, int height) {
        obj = Instantiate(obj, new Vector2(width, height), Quaternion.identity);
        obj.transform.parent = this.transform;
    }
}