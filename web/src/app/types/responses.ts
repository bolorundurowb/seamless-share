import { AppSource } from './enums';

export interface UserRes {
  id: string,
  firstName?: string;
  lastName?: string;
  emailAddress: string;
  joinedAt: string;
}

export interface AuthRes {
  accessToken: string;
  expiresAt: string;
  user: UserRes;
}

export interface GenericMessageRes {
  message: string;
}

export interface ShareRes {
  id: string;
  code: string;
  createdAt: string;
}

export interface FileRes {
  id: string;
  url: string;
  createdAt: string;
  updatedAt: string;
  source?: AppSource;
  deviceName?: string;
  appVersion?: string;
  metadata: {
    externalId: string;
    name: string;
    extension: string;
    sizeInBytes: number;
    mimeType: string;
    checksum?: string;
  };
}

export interface LinkRes {
  id: string;
  url: string;
  createdAt: string;
  updatedAt: string;
  source?: AppSource;
  deviceName?: string;
  appVersion?: string;
}

export interface TextRes {
  id: string;
  content: string;
  createdAt: string;
  updatedAt: string;
  source?: AppSource;
  deviceName?: string;
  appVersion?: string;
}
