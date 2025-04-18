export const isFile = (item? : any): boolean => item && item.url && item.metadata?.mimeType;

export const isLink = (item? : any): boolean => item && item.url && !item.metadata?.mimeType;


export const isText = (item? : any): boolean => item && item.content;
