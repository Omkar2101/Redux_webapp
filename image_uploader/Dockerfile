# Stage 1: Build the React app
FROM node:18.14.2 AS build
WORKDIR /app

# Copy package.json and package-lock.json first to leverage caching
COPY package.json package-lock.json ./

# Install dependencies using npm ci for clean installs
RUN npm ci

# Copy the entire frontend source code
COPY . .

# Build the frontend
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Remove default Nginx static files
RUN rm -rf ./*

# Copy built frontend files from the previous build stage
COPY --from=build /app/dist ./

# Expose port for frontend
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
