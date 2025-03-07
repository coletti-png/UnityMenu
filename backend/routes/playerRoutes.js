const express = require("express");
const { nanoid } = require("nanoid");
const Player = require("../models/Player");

const router = express.Router();

// Get all players sorted alphabetically
router.get("/", async (req, res) => {
    try {
        const players = await Player.find().sort({ ScreenName: 1 });
        res.json(players);
    } catch (error) {
        res.status(500).json({ error: "Failed to retrieve players" });
    }
});

// Get player by screen name
router.get("/:screenName", async (req, res) => {
    try {
        const player = await Player.findOne({ ScreenName: new RegExp(`^${req.params.screenName}$`, "i") });

        if (!player) {
            return res.status(404).json({ error: "Player not found" });
        }
        res.json(player);
    } catch (error) {
        res.status(500).json({ error: "Failed to retrieve player" });
    }
});

// Create a new player entry
router.post("/", async (req, res) => {
    try {
        const { ScreenName, FirstName, LastName, DateStartedPlaying, Score } = req.body;

        if (!ScreenName || !FirstName || !LastName || !Score) {
            return res.status(400).json({ error: "Missing required fields" });
        }

        const newPlayer = new Player({
            playerid: nanoid(8),
            ScreenName,
            FirstName,
            LastName,
            DateStartedPlaying: DateStartedPlaying || new Date().toISOString().split("T")[0],
            Score
        });

        await newPlayer.save();
        res.json({ message: "Player added successfully", playerid: newPlayer.playerid });
    } catch (error) {
        res.status(500).json({ error: "Failed to add player" });
    }
});

// Edit player data
router.put("/:playerid", async (req, res) => {
    try {
        const { ScreenName, FirstName, LastName, Score } = req.body;
        const updatedPlayer = await Player.findOneAndUpdate(
            { playerid: req.params.playerid },
            { ScreenName, FirstName, LastName, Score },
            { new: true }
        );

        if (!updatedPlayer) {
            return res.status(404).json({ error: "Player not found" });
        }

        res.json({ message: "Player updated successfully", player: updatedPlayer });
    } catch (error) {
        res.status(500).json({ error: "Failed to update player" });
    }
});

// Delete player
router.delete("/:playerid", async (req, res) => {
    try {
        const deletedPlayer = await Player.findOneAndDelete({ playerid: req.params.playerid });
        if (!deletedPlayer) {
            return res.status(404).json({ error: "Player not found" });
        }
        res.json({ message: "Player deleted successfully" });
    } catch (error) {
        res.status(500).json({ error: "Failed to delete player" });
    }
});

module.exports = router;
