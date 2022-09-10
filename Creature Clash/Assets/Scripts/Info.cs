using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Info 
{
    public static string[] animals = {"Snake", "Rabbit", "Shark", "Spider", "Crocodile", "Lion", "Wolf", "Beaver", "Cow", "Jellyfish", "Chicken", "Ant"};
    public static string[] deck = {"Snake", "Beaver", "Cow", "Jellyfish", "Ant", "Spider", "Crocodile", "Lion"};
    public static Dictionary<string, Dictionary<string, int>> stats = new Dictionary<string, Dictionary<string, int>> {
        ["Egg"] = new Dictionary<string, int> {
            ["mana"] = 0, ["hp"] = 100, ["atk"] = 0, ["spd"] = 0, 
        },
        ["Snake"] = new Dictionary<string, int> {
            ["mana"] = 4, ["hp"] = 20, ["atk"] = 5, ["spd"] = 25, 
        },
        ["Rabbit"] = new Dictionary<string, int> {
            ["mana"] = 2, ["hp"] = 15, ["atk"] = 2, ["spd"] = 40,
        },
        ["Shark"] = new Dictionary<string, int> {
            ["mana"] = 6, ["hp"] = 40, ["atk"] = 10, ["spd"] = 20,
        },
        ["Spider"] = new Dictionary<string, int> {
            ["mana"] = 2, ["hp"] = 10, ["atk"] = 5, ["spd"] = 30,
        },
        ["Crocodile"] = new Dictionary<string, int> {
            ["mana"] = 3, ["hp"] = 25, ["atk"] = 6, ["spd"] = 25,
        },
        ["Lion"] = new Dictionary<string, int> {
            ["mana"] = 6, ["hp"] = 35, ["atk"] = 6, ["spd"] = 25,
        },
        ["Wolf"] = new Dictionary<string, int> {
            ["mana"] = 3, ["hp"] = 25, ["atk"] = 8, ["spd"] = 25,
        },
        ["Beaver"] = new Dictionary<string, int> {
            ["mana"] = 2, ["hp"] = 20, ["atk"] = 3, ["spd"] = 20,
        },
        ["Cow"] = new Dictionary<string, int> {
            ["mana"] = 4, ["hp"] = 45, ["atk"] = 3, ["spd"] = 15,
        },
        ["Jellyfish"] = new Dictionary<string, int> {
            ["mana"] = 3, ["hp"] = 20, ["atk"] = 3, ["spd"] = 25,
        },
        ["Chicken"] = new Dictionary<string, int> {
            ["mana"] = 3, ["hp"] = 5, ["atk"] = 5, ["spd"] = 35,
        },
        ["Ant"] = new Dictionary<string, int> {
            ["mana"] = 6, ["hp"] = 1, ["atk"] = 1, ["spd"] = 30,
        },
    };
    public static Dictionary<string, string> desc = new Dictionary<string, string> {
        ["Egg"] = "Egg", 
        ["Snake"] = "Venom: Poisons enemies hit for 3 turns.", 
        ["Rabbit"] = "Hoppy: Gain speed when carrot hits an enemy. \nVitamin A - 2 mana: Throw carrot.",
        ["Shark"] = "Just a swimmer: Doesn't lose speed in water. Loses 3 hp if out of water at end of turn.",
        ["Spider"] = "Stealthy: Invisible until hit. \nWeb Trap - 2 mana: Place invisible web. Enemies on it are slowed and take 2x dmg from Spider.",
        ["Crocodile"] = "Swimmer: Doesn't lose speed in water. \nLunge - 2 mana: If in a pond, can lunge in a direction, dragging first enemy hit back to water.",
        ["Lion"] = "King of the Jungle: Cuts nearby enemies' ATK in half. \nRoar - 1 mana: Nearby enemies flee Lion.",
        ["Wolf"] = "Sigma: Loses 1 ATK per nearby ally",
        ["Beaver"] = "Dam son - 2 mana: Builds a dam that gets destroyed on impact.",
        ["Cow"] = "Magic Milk: Allies hit will heal for 3 turns.",
        ["Jellyfish"] = "Just a swimmer: Doesn't lose speed in water. Loses 3 hp if out of water at end of turn. \nZap: Enemies hit are paralyzed for 3 turns.",
        ["Chicken"] = "Undying Headless Chicken: Can't die. \nUseless Headless Chicken: ATK = HP.",
        ["Ant"] = "Hard Work: Generates 1 mana per turn. \nPower of the Colony: ATK is equal to # of ally Ants x 3.",
    };
}
