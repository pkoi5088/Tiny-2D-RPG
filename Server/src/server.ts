import http from 'http';
import { Server } from 'socket.io';
import app from './app';

const server = http.createServer(app);
const io = new Server(server);

io.on('connection', (socket) => {
    console.log('User connected:', socket.id);
    
    socket.on('message', (data) => {
        console.log('Received message:', data);
    });

    socket.on('disconnect', () => {
        console.log('User disconnected:', socket.id);
    });
});

const PORT = 3000;
server.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});
