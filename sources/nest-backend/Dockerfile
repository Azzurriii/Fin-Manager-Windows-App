FROM node:18-alpine

WORKDIR /usr/src/app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy source code
COPY . .

# Build application
RUN npm run build

# Expose port 3000 for the Nest.js application
EXPOSE 3000

# Start the application
CMD ["npm", "run", "start"]