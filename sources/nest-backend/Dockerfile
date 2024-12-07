FROM node:18-alpine

WORKDIR /usr/src/app

# Install netcat
RUN apk add --no-cache netcat-openbsd

# Copy package.json first
COPY package*.json ./

# Install dependencies
RUN yarn install

# Copy source code
COPY . .

# Build application
RUN yarn build

EXPOSE 3000

CMD ["yarn", "start:prod"]