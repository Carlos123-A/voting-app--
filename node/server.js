const express = require('express');
const { Pool } = require('pg');
const WebSocket = require('ws');
const path = require('path');

const app = express();
const port = 3000;

const pool = new Pool({
    user: 'postgres',
    host: 'db', 
    database: 'mydb',
    password: 'yourpassword',
    port: 5432,
});

app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'index.html'));
});

const server = app.listen(port, () => {
    console.log(`Servidor escuchando en el puerto ${port}`);
});

const wss = new WebSocket.Server({ server });

wss.on('connection', ws => {
    console.log('Cliente conectado a WebSocket');

    sendResultsToClient(ws);

    const interval = setInterval(() => {
        sendResultsToClient(ws);
    }, 5000);

    ws.on('close', () => {
        clearInterval(interval);
        console.log('Cliente desconectado de WebSocket');
    });
});

async function sendResultsToClient(ws) {
    try {
        const res = await pool.query('SELECT cats_votes, dogs_votes FROM votes WHERE id = 1');
        const results = res.rows[0];

        ws.send(JSON.stringify({
            catsVotes: results.cats_votes,
            dogsVotes: results.dogs_votes,
        }));
    } catch (err) {
        console.error('Error obteniendo datos de PostgreSQL:', err);
    }
}
