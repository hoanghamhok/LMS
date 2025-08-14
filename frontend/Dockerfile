# ========== 1) Cài deps ==========
FROM node:18-alpine AS deps
WORKDIR /app

# Cần có cả package.json + package-lock.json trong repo
COPY package.json package-lock.json ./
RUN npm ci

# ========== 2) Build ==========
FROM node:18-alpine AS build
WORKDIR /app

# Copy node_modules đã cài ở stage deps
COPY --from=deps /app/node_modules ./node_modules

# Copy toàn bộ source
COPY . .

# Build Next.js (production)
RUN npm run build

# ========== 3) Runtime ==========
FROM node:18-alpine AS run
WORKDIR /app
ENV NODE_ENV=production
ENV PORT=3000

# Copy project đã build
COPY --from=build /app ./

EXPOSE 3000
# Next.js dùng `next start` trong npm scripts
CMD ["npm", "run", "start"]
