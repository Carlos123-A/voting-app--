FROM node:14

WORKDIR /app

RUN npm init -y

RUN npm install express pg ws

COPY server.js /app/server.js
COPY index.html /app/index.html

EXPOSE 3000

CMD ["node", "server.js"]
